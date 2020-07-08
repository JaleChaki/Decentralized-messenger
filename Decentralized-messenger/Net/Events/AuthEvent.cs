using Newtonsoft.Json;

namespace Messenger.Net.Events {
	public class AuthEvent : Event {

		public const string EventUniqueId = "auth";

		[JsonConstructor]
		public AuthEvent(string fromId) : base(fromId, EventUniqueId) {
			
		}

	}
}
