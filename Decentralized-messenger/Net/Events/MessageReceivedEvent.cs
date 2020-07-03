namespace Messenger.Net.Events {
	public class MessageReceivedEvent : Event {

		public string MessageContent { get; }

		public MessageReceivedEvent(string fromId, string content) : base(fromId) {
			MessageContent = content;
		}

	}
}
