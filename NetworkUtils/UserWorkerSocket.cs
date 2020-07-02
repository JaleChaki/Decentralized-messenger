using System.Net.Sockets;

namespace NetworkUtils {
	internal class UserWorkerSocket {

		public readonly string ClientId;

		public readonly Socket Socket;

		public bool Connected => Socket == null ? false : Socket.Connected;

		public int EmptyDataReceived { get; set; } = 0;

		public UserWorkerSocket(string clientId, Socket s) {
			ClientId = clientId;
			Socket = s;
		}

		public void CloseConnection() {
			Socket?.Close();
		}

	}
}
