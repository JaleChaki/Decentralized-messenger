namespace Messenger.Net.Events {

	public abstract class Event {

		public string FromId;

		public string EventType;

		public Event(string fromId, string eventType) {
			this.FromId = fromId;
			this.EventType = eventType;
		}

	}
}
