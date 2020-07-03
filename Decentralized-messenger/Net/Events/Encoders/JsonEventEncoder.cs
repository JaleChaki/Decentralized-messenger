using System.Text;
using Newtonsoft.Json;

namespace Messenger.Net.Events.Encoders {
	public class JsonEventEncoder : IEventEncoder {

		private static readonly Encoding EncodeMethod = Encoding.UTF8;

		private class TempEvent {
			public string EventType { get; set; }
		}

		public Event Decode(byte[] data) {
			string jsonStr = EncodeMethod.GetString(data);
			TempEvent e = JsonConvert.DeserializeObject<TempEvent>(jsonStr);
			if (e.EventType == MessageReceivedEvent.EventUniqueId) {
				return JsonConvert.DeserializeObject<MessageReceivedEvent>(jsonStr);
			}
			return null;
		}

		public byte[] Encode(Event e) {
			string jsonStr = JsonConvert.SerializeObject(e);
			return EncodeMethod.GetBytes(jsonStr);
		}
	}
}
