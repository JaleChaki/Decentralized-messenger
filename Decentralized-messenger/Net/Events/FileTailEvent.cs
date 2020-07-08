using Newtonsoft.Json;

namespace Messenger.Net.Events {
	public class FileTailEvent : Event {

		public const string EventUniqueId = "ftail";

		[JsonConstructor]
		public FileTailEvent(string fromId) : base(fromId, EventUniqueId) {

		}

	}
}
