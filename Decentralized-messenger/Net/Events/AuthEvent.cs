namespace Messenger.Net.Events {
	public class AuthEvent : Event {

		public const string EventUniqueId = "auth";

		public AuthEvent(string fromId) : base(fromId, EventUniqueId) {
			
		}

	}
}
