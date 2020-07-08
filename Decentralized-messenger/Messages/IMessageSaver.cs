using System;
using System.Collections.Generic;

namespace Messenger.Messages {
	public interface IMessageSaver {

		void SaveMessage(string fromId, string messageContent);

		IEnumerable<Tuple<string, string>> LoadMessages();

	}
}
