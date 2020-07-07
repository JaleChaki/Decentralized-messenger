using Messenger.Net.Events;
using System.Collections.Generic;

namespace Messenger.Net {
	public interface INetworkController {

		IEnumerable<string> OnlineClients { get; }

		void Start();

		void SendMessage(string messageText, string userId);

		void SendFile(string filePath, string userId);

		void Update();

		IEnumerable<Event> GetUpdates();

	}
}
