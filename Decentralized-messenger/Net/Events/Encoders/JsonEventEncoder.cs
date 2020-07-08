using System.Text;
using Newtonsoft.Json;
using XLogger;

namespace Messenger.Net.Events.Encoders {
	public class JsonEventEncoder : IEventEncoder {

		private static readonly Encoding EncodeMethod = Encoding.UTF8;

		private class TempEvent {
			public string EventType { get; set; }
		}

		public Event Decode(byte[] data) {
			string jsonStr = EncodeMethod.GetString(data);
			Logger.Debug("decode string " + jsonStr);
			TempEvent e = JsonConvert.DeserializeObject<TempEvent>(jsonStr);
			if (e.EventType == MessageReceivedEvent.EventUniqueId) {
				return JsonConvert.DeserializeObject<MessageReceivedEvent>(jsonStr);
			}
			if (e.EventType == AuthEvent.EventUniqueId) {
				return JsonConvert.DeserializeObject<AuthEvent>(jsonStr);
			}
			return null;
		}

		public byte[] Encode(Event e) {
			StringBuilder jsonStr = new StringBuilder(JsonConvert.SerializeObject(e));
			while (jsonStr.Length < 1024) {
				jsonStr.Append(' ');
			}
			return EncodeMethod.GetBytes(jsonStr.ToString());
		}
	}
}
