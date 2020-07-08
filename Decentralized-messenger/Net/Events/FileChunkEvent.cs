using Newtonsoft.Json;

namespace Messenger.Net.Events {
	public class FileChunkEvent : Event {

		public const string EventUniqueId = "fchunk";

		[JsonProperty("b")]
		public string Body;

		[JsonConstructor]
		private FileChunkEvent() : base("", EventUniqueId) {

		}

		public FileChunkEvent(string fromId, string body) : base(fromId, EventUniqueId) {
			this.Body = body;
		}

	}
}
