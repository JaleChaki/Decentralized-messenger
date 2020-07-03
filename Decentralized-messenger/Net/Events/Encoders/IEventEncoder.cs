namespace Messenger.Net.Events.Encoders {
	public interface IEventEncoder {

		byte[] Encode(Event e);

		Event Decode(byte[] data);

	}
}
