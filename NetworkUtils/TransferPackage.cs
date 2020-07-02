using System.Net.Sockets;

namespace NetworkUtils {
	internal class TransferPackage {

		public Socket CurrentSocket { get; set; }

		public string ClientId { get; set; }

		public byte[] Buffer = new byte[1024];

		public TransferPackage(Socket currentSocket, string clientId) {
			ClientId = clientId;
			CurrentSocket = currentSocket;
		}

	}
}
