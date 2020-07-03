using System.Net.Sockets;

namespace NetworkUtils {
	internal class ClientTransferPackage {

		public Socket CurrentSocket { get; set; }


		public byte[] Buffer = new byte[1024];

		public ClientTransferPackage(Socket currentSocket) {
			CurrentSocket = currentSocket;
		}

	}
}
