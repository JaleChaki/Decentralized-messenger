using Messenger.Net.Events;
using Messenger.Net.Events.Encoders;
using Messenger.Net.Scanners;
using NetworkUtils;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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

		public DefaultNetworkController(string selfId) {
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
			this.SelfId = selfId;
		}

		public void UpdateNetwork() {
			IPAddress[] ips = NetworkScanner.Scan().ToArray();
			bool[] exists = new bool[ips.Length];
			foreach (var c in Clients.Values) {
				for (int i = 0; i < ips.Length; ++i) {
					if (ips[i].ToString() == c.ConnectedAddress.ToString()) {
						exists[i] = true;
					}
				}
			}
			for (int i = 0; i < ips.Length; ++i) {
				if (exists[i]) {
					continue;
				}
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
				if (UnauthClients.ContainsKey(id)) {
					if (e is AuthEvent) {
						AsyncNetworkClient client = UnauthClients[id];
						UnauthClients.Remove(id);
						Clients.Add(e.FromId, client);
					}
				}
			}
		}

		public void ClientDisconnect(string id) {
			if (UnauthClients.ContainsKey(id)) {
				UnauthClients[id].OnClientReceiveData -= ClientReceiveData;
				UnauthClients[id].OnClientDisconnected -= ClientDisconnect;
				UnauthClients.Remove(id);
			}
			if (Clients.ContainsKey(id)) {
				Clients[id].OnClientReceiveData -= ClientReceiveData;
				Clients[id].OnClientDisconnected -= ClientDisconnect;
				Clients.Remove(id);
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
			Server.SendData(clientId, EventEncoder.Encode(new AuthEvent(SelfId)));
		}

		private void ServerDataReceive(string clientId, byte[] data, int dataSize) {
			lock (Sync) {
				byte[] transformedData = new byte[dataSize];
				for (int i = 0; i < dataSize; ++i) {
					transformedData[i] = data[i];
				}
				Event e = EventEncoder.Decode(transformedData);
				if (e is AuthEvent) {
					if (ClientNameToId.ContainsKey(e.FromId)) {
						ClientNameToId.Remove(e.FromId);
					}
					ClientNameToId.Add(e.FromId, clientId);
					return;
				}
				ReceivedEvents.Add(EventEncoder.Decode(transformedData));
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
			
		}

		public void SendMessage(string messageText, string userId) {
			Clients[userId].SendData(EventEncoder.Encode(new MessageReceivedEvent(SelfId, messageText)));
		}

		public void Start() {
			Server.StartListen(IPAddress.Any, ListenPort, 10);
		}
	}
}
