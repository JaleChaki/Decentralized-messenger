namespace Messenger.Net.Events {

	public class MessageReceivedEvent : Event {

		public const string EventUniqueId = "msgReceive";

		public string MessageContent { get; }

		public MessageReceivedEvent(string fromId, string content) : base(fromId, EventUniqueId) {
			MessageContent = content;
		}

	}
}
