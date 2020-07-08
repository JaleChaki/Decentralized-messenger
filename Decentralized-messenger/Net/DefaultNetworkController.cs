using Messenger.Config;
using Messenger.Net.Events;
using Messenger.Net.Events.Encoders;
using Messenger.Net.FileConverters;
using Messenger.Net.Scanners;
using NetworkUtils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using XLogger;

namespace Messenger.Net {
	public class DefaultNetworkController : INetworkController {

		private readonly AsyncNetworkListener Server;

		private readonly IDictionary<string, AsyncNetworkClient> Clients;

		private readonly IDictionary<string, AsyncNetworkClient> UnauthClients;

		private readonly IDictionary<string, string> ClientNameToId;

		private readonly IEventEncoder EventEncoder;

		private readonly INetworkScanner NetworkScanner;

		private ICollection<Event> ReceivedEvents;

		private string SelfId;

		private object Sync = new object();

		private int ConnectPort = 1020;

		private int ListenPort = 1020;

		private Stream FileStream;

		public IEnumerable<string> OnlineClients {
			get {
				lock (Sync) {
					foreach (var c in Clients.Keys) {
						yield return c;
					}
				}
			}
		}

		public DefaultNetworkController(NetworkConfig config) {
			this.Server = new AsyncNetworkListener();
			this.Clients = new Dictionary<string, AsyncNetworkClient>();
			this.UnauthClients = new Dictionary<string, AsyncNetworkClient>();
			this.ClientNameToId = new Dictionary<string, string>();
			this.Server.OnClientConnect += ServerConnect;
			this.Server.OnClientReceiveData += ServerDataReceive;
			this.Server.OnClientDisconnect += ServerDisconnect;
			this.EventEncoder = new JsonEventEncoder();
			// TODO implement working scanner
			this.NetworkScanner = new TestNetworkScanner();
			this.SelfId = config.SelfId;
			this.ConnectPort = config.ConnectPort;
			this.ListenPort = config.ListenPort;
			this.ReceivedEvents = new List<Event>();
		}

		public void Update() {
			IPAddress[] ips = NetworkScanner.Scan().ToArray();
			bool[] exists = new bool[ips.Length];
			lock (Sync) {
				foreach (var c in Clients.Values) {
					for (int i = 0; i < ips.Length; ++i) {
						if (ips[i].ToString() == c.ConnectedAddress?.ToString()) {
							exists[i] = true;
						}
					}
				}
			}
			for (int i = 0; i < ips.Length; ++i) {
				if (exists[i]) {
					continue;
				}
				Logger.Debug("new ip detected, attempt to create new client session");
				CreateAsyncClient(ips[i]);
			}
		}

		private void CreateAsyncClient(IPAddress address) {
			lock (Sync) {
				AsyncNetworkClient client = new AsyncNetworkClient(IdGenerator.CreateId());
				client.OnClientReceiveData += ClientReceiveData;
				client.OnClientDisconnected += ClientDisconnect;
				client.Connect(address, ConnectPort);
				UnauthClients.Add(client.Id, client);
			}
		}

		private void ClientReceiveData(string id, byte[] data, int size) {
			lock (Sync) {
				byte[] transformedData = new byte[size];
				for (int i = 0; i < size; ++i) {
					transformedData[i] = data[i];
				}
				Event e = EventEncoder.Decode(transformedData);
				Logger.Debug("received message from remote server, event type " + e.EventType);
				if (UnauthClients.ContainsKey(id)) {
					if (e is AuthEvent) {
						AsyncNetworkClient client = UnauthClients[id];
						UnauthClients.Remove(id);
						if (Clients.ContainsKey(e.FromId)) {
							Clients[e.FromId].OnClientDisconnected -= ClientDisconnect;
							Clients[e.FromId].OnClientReceiveData -= ClientReceiveData;
							Clients.Remove(e.FromId);
						}
						Logger.Debug("auth complete code = " + e.FromId);
						Clients.Add(e.FromId, client);
					}
				} else {
					Logger.Debug("skipped");
				}
			}
		}

		public void ClientDisconnect(string id) {
			lock (Sync) { 
				if (UnauthClients.ContainsKey(id)) {
					Logger.Debug("Disconnected unknown client " + id);
					UnauthClients[id].OnClientReceiveData -= ClientReceiveData;
					UnauthClients[id].OnClientDisconnected -= ClientDisconnect;
					UnauthClients.Remove(id);
				}
				Logger.Debug("here");
				string removingKey = null;
				foreach (var c in Clients) {
					if (c.Value.Id == id) {
						removingKey = c.Key;
					}
				}
				if (removingKey == null) {
					return;
				}
				Clients.Remove(removingKey);
			}
		}

		private void RemoveClient(string clientId) {
			string clientName = null;
			foreach (var p in ClientNameToId) {
				if (p.Value == clientId) {
					clientName = p.Key;
				}
			}
			if (clientName == null) {
				return;
			}
			ClientNameToId.Remove(clientName);
		}

		private void ServerConnect(string clientId) {
			Logger.Debug("connected to remote client, send code " + SelfId);
			Server.SendData(clientId, EventEncoder.Encode(new AuthEvent(SelfId)));
		}

		private void ServerDataReceive(string clientId, byte[] data, int dataSize) {
			lock (Sync) {
				byte[] transformedData = new byte[dataSize];
				for (int i = 0; i < dataSize; ++i) {
					transformedData[i] = data[i];
				}
				Event e = EventEncoder.Decode(transformedData);
				Logger.Debug("received message from remote client, event type " + e.EventType);
				if (e is AuthEvent) {
					if (ClientNameToId.ContainsKey(e.FromId)) {
						ClientNameToId.Remove(e.FromId);
					}
					Logger.Debug("auth complete code = " + e.FromId);
					ClientNameToId.Add(e.FromId, clientId);
					return;
				}
				if (e is MessageReceivedEvent) {
					ReceivedEvents.Add(EventEncoder.Decode(transformedData));
				}
				if (e is FileHeaderEvent fhead) {
					FileStream?.Close();
					FileStream = new FileStream(fhead.Filename, FileMode.Create);

				}
				if (e is FileChunkEvent fchunk) {
					IFileConverter conv = new BinaryConverter();
					var chunk = conv.DecodeString(fchunk.Body);
					FileStream.Write(chunk, 0, chunk.Length);
				}
				if (e is FileTailEvent ftail) {
					FileStream?.Close();
					FileStream = null;
				}
			}
		}

		private void ServerDisconnect(string clientId) {
			lock (Sync) {
				RemoveClient(clientId);
			}
		}

		public IEnumerable<Event> GetUpdates() {
			lock (Sync) {
				foreach (var e in ReceivedEvents) {
					yield return e;
				}
				ReceivedEvents.Clear();
			}
		}

		public void SendFile(string filePath, string userId) {
			lock (Sync) {
				if (Clients.ContainsKey(userId)) {
					Clients[userId].SendData(EventEncoder.Encode(new FileHeaderEvent(SelfId, Path.GetFileName(filePath))));
					byte[] file = File.ReadAllBytes(filePath);
					IFileConverter conv = new BinaryConverter();
					int ykz = 0;
					while (ykz < file.Length) {
						int i = 0;
						StringBuilder senddata = new StringBuilder();
						for (; i < 200 && ykz < file.Length; ++i, ++ykz) {
							senddata.Append(conv.EncodeBytes(new byte[1] { file[ykz] }));
						}
						Clients[userId].SendData(EventEncoder.Encode(new FileChunkEvent(SelfId, senddata.ToString())));
					}
					Clients[userId].SendData(EventEncoder.Encode(new FileTailEvent(SelfId)));
				}
			}
		}

		public void SendMessage(string messageText, string userId) {
			Logger.Debug("send message to " + userId);
			if (Clients.ContainsKey(userId)) {
				Clients[userId].SendData(EventEncoder.Encode(new MessageReceivedEvent(SelfId, messageText)));
				lock (Sync) {
					ReceivedEvents.Add(new MessageReceivedEvent(SelfId, messageText));
				}
			} else {
				Logger.Debug("cancelled");
			}
		}

		public void Start() {
			Server.StartListen(IPAddress.Any, ListenPort, 10);
		}
	}
}
