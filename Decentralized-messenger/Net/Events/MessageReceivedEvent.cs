using Newtonsoft.Json;

namespace Messenger.Net.Events {

	public class MessageReceivedEvent : Event {

		public const string EventUniqueId = "msgReceive";

		[JsonProperty]
		public string MessageContent { get; private set; }

		public MessageReceivedEvent(string fromId, string content) : base(fromId, EventUniqueId) {
			MessageContent = content;
		}

	}
}
