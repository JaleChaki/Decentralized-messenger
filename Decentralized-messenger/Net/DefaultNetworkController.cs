using Messenger.Net.Events;
using NetworkUtils;
using System.Collections.Generic;
using System.Net;

namespace Messenger.Net {
	public class DefaultNetworkController : INetworkController {

		private AsyncNetworkListener Server;

		private IDictionary<string, AsyncNetworkClient> Clients;

		public DefaultNetworkController() {
			Server = new AsyncNetworkListener();
			Clients = new Dictionary<string, AsyncNetworkClient>();
		}

		public IEnumerable<Event> GetUpdates() {
			yield break;
		}

		public void SendFile(string filePath, string userId) {
			
		}

		public void SendMessage(string messageText, string userId) {
			//Clients[userId].SendData()
		}

		public void Start() {
			Server.StartListen(IPAddress.Any, 1020, 10);
		}
	}
}
