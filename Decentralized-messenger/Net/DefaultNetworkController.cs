using Messenger.Net.Events;
using Messenger.Net.Events.Encoders;
using NetworkUtils;
using System.Collections.Generic;
using System.Net;

namespace Messenger.Net {
	public class DefaultNetworkController : INetworkController {

		private readonly AsyncNetworkListener Server;

		private readonly IDictionary<string, AsyncNetworkClient> Clients;

		private readonly IEventEncoder EventEncoder;

		private ICollection<Event> ReceivedEvents;

		private string SelfId;

		public DefaultNetworkController(string selfId) {
			Server = new AsyncNetworkListener();
			Clients = new Dictionary<string, AsyncNetworkClient>();
			Server.OnClientReceiveData += ClientDataReceive;
			EventEncoder = new JsonEventEncoder();
			this.SelfId = selfId;
		}

		private void ClientDataReceive(string clientId, byte[] data, int dataSize) {
			byte[] transformedData = new byte[dataSize];
			for (int i = 0; i < dataSize; ++i) {
				transformedData[i] = data[i];
			}
			ReceivedEvents.Add(EventEncoder.Decode(transformedData));
		}

		public IEnumerable<Event> GetUpdates() {
			foreach (var e in ReceivedEvents) {
				yield return e;
			}
			ReceivedEvents.Clear();
		}

		public void SendFile(string filePath, string userId) {
			
		}

		public void SendMessage(string messageText, string userId) {
			Clients[userId].SendData(EventEncoder.Encode(new MessageReceivedEvent(SelfId, messageText)));
		}

		public void Start() {
			Server.StartListen(IPAddress.Any, 1020, 10);
		}
	}
}
