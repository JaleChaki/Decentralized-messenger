namespace Messenger.Net.FileConverters {

	public interface IFileConverter {

		string EncodeBytes(byte[] data);

		byte[] DecodeString(string converted);

		int GetEncodedSize(byte b);

	}
}
