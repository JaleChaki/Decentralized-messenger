using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Messages {
	public interface IMessageSaver {

		void SaveMessage(string fromId, string messageContent);

		IDictionary<string, IEnumerable<string>> LoadMessages();

	}
}
