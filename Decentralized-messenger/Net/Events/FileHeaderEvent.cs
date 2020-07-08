using Newtonsoft.Json;

namespace Messenger.Net.Events {
	public class FileHeaderEvent : Event {

		public const string EventUniqueId = "fhead";

		[JsonProperty]
		public string Filename { get; private set; }

		[JsonConstructor]
		private FileHeaderEvent() : base("", EventUniqueId) {

		}

		public FileHeaderEvent(string fromId, string filename) : base(fromId, EventUniqueId) {
			this.Filename = filename;
		}

	}
}
