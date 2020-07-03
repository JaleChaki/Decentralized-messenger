using Messenger.Net.Events;
using System.Collections.Generic;

namespace Messenger.Net {
	public interface INetworkController {

		void Start();

		void SendMessage(string messageText, string userId);

		void SendFile(string filePath, string userId);

		IEnumerable<Event> GetUpdates();

	}
}
