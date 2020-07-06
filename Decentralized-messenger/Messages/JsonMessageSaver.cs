using System;
using System.Collections.Generic;

namespace Messenger.Messages {
	public class JsonMessageSaver : IMessageSaver {
		
		private const string FileLocation = "messages.json";
		
		public IDictionary<string, IEnumerable<string>> LoadMessages() {
			throw new NotImplementedException();
		}

		public void SaveMessage(string fromId, string messageContent) {
			
		}
	}
}
