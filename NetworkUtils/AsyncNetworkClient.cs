using System;
using System.Net;
using System.Net.Sockets;
using XLogger;

namespace NetworkUtils {
	public class AsyncNetworkClient {

		public Socket ClientSocket { get; private set; }

		public delegate void ReceiveDataCallback(byte[] data, int size);

		public ReceiveDataCallback OnClientReceiveData { get; set; }

		public delegate void ClientDisconnectCallback();

		public ClientDisconnectCallback OnClientDisconnected { get; set; }

		public bool Connected => ClientSocket == null ? false : ClientSocket.Connected;

		public AsyncNetworkClient() {

		}

		public void Connect(IPAddress address, int port) {
			try {
				Disconnect();
				ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				ClientSocket.Connect(new IPEndPoint(address, port));
				if (ClientSocket.Connected) {
					WaitForData();
				}
			}
			catch (SocketException se) {
				Logger.Error("Client exception in Connect: " + se.Message);
			}
		}

		public void Disconnect() {
			ClientSocket?.Close();
		}

		private void WaitForData() {
			try {
				ClientTransferPackage package = new ClientTransferPackage(ClientSocket);
				ClientSocket.BeginReceive(package.Buffer, 0, package.Buffer.Length, SocketFlags.None, new AsyncCallback(OnDataReceived), package);
			}
			catch (SocketException se) {
				Logger.Error("Client exception in WaitForData: " + se.Message);
			}
		}

		private void OnDataReceived(IAsyncResult asyn) {
			try {
				TransferPackage socketData = (TransferPackage)asyn.AsyncState;
				int dataSize = socketData.CurrentSocket.EndReceive(asyn);

				OnClientReceiveData?.Invoke(socketData.Buffer, dataSize);
				WaitForData();
			}
			catch (ObjectDisposedException) {
				Logger.Error("Client exception in OnDataReceived: Socket has been closed");
			}
			catch (SocketException se) {
				Logger.Error("Client exception in OnDataReceived: " + se.Message);

				OnClientDisconnected?.Invoke();
			}
		}

		public void SendData(byte[] data) {
			if (Connected) {
				try {
					ClientSocket.Send(data);
				}
				catch (SocketException) {

				}
			}
		}
	}
}
