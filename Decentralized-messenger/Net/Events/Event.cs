namespace Messenger.Net.Events {

	public abstract class Event {

		public string FromId;

		public Event(string fromId) {
			this.FromId = fromId;
		}

	}
}
