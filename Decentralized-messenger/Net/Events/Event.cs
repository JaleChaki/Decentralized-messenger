using Newtonsoft.Json;

namespace Messenger.Net.Events {

	public abstract class Event {

		[JsonProperty]
		public string FromId { get; private set; }

		[JsonProperty]
		public string EventType { get; private set; }

		public Event(string fromId, string eventType) {
			this.FromId = fromId;
			this.EventType = eventType;
		}

	}
}
