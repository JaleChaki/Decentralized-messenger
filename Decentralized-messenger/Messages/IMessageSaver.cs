using System.Collections.Generic;

namespace Messenger.Messages {
	public interface IMessageSaver {

		void SaveMessage(string fromId, string messageContent);

		IDictionary<string, IEnumerable<string>> LoadMessages();

	}
}
