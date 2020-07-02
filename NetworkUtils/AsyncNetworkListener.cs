using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using XLogger;

namespace NetworkUtils {
	public class AsyncNetworkListener{

		/// <summary>
		/// set of client sockets
		/// </summary>
		private Dictionary<string, UserWorkerSocket> WorkerSockets;

		private Socket MainSocket;

		public bool IsListening => MainSocket == null ? false : MainSocket.IsBound;

		/// <summary>
		/// callback, which called on finishing client connection
		/// </summary>
		/// <param name="clientId"></param>
		public delegate void ClientConnectCallback(string clientId);
		public ClientConnectCallback OnClientConnect { get; set; } = null;

		/// <summary>
		/// callback, which called on finishing data receiving
		/// </summary>
		/// <param name="clientId"></param>
		/// <param name="data"></param>
		/// <param name="dataSize"></param>
		public delegate void ClientReceiveDataCallback(string clientId, byte[] data, int dataSize);
		public ClientReceiveDataCallback OnClientReceiveData { get; set; } = null;

		/// <summary>
		/// callback, which called on client disconnection
		/// </summary>
		/// <param name="clientId"></param>
		public delegate void ClientDisconnectionCallback(string clientId);
		public ClientDisconnectionCallback OnClientDisconnect { get; set; } = null;

		public int ClientMaxBadPackets { get; set; }

		public AsyncNetworkListener() {
			WorkerSockets = new Dictionary<string, UserWorkerSocket>();
			OnClientDisconnect += FinalizeClientDisconnection;
		}

		public bool StartListen(IPAddress address, int listenPort, int maxConnections) {
			try {
				StopListen();
				if (address == IPAddress.Any) {
					Logger.Warn("TCP Listener will start and listen on all network interfaces");
				}
				MainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				MainSocket.Bind(new IPEndPoint(address, listenPort));
				MainSocket.Listen(maxConnections);
				MainSocket.BeginAccept(new AsyncCallback(OnReceiveConnection), null);
				Logger.Info("TCP Listener started");
				return true;
			}
			catch (SocketException se) {
				Logger.Error("Can't starting TCP Listener: " + se.Message);
				return false;
			}
		}

		public void StopListen() {
			lock (WorkerSockets) {
				foreach (UserWorkerSocket s in WorkerSockets.Values) {
					if (s.Connected) {
						s.CloseConnection();
					}
				}
				WorkerSockets.Clear();
			}
			if (IsListening) {
				MainSocket.Close();
			}
		}

		private void FinalizeClientDisconnection(string clientId) {
			if (WorkerSockets.ContainsKey(clientId)) {
				Logger.Info("Client " + WorkerSockets[clientId].Socket.RemoteEndPoint.ToString() + " disconnected, close session " + clientId);
				lock (WorkerSockets) {
					WorkerSockets.Remove(clientId);
				}
			}
		}

		private void OnReceiveConnection(IAsyncResult res) {
			try {
				string newClientId = IdGenerator.CreateId();
				lock (WorkerSockets) {
					var newWorkerSocket = new UserWorkerSocket(newClientId, MainSocket.EndAccept(res));
					WorkerSockets.Add(newWorkerSocket.ClientId, newWorkerSocket);
					Logger.Info("Connected client " + newWorkerSocket.Socket.RemoteEndPoint.ToString() + ", create session with id " + newWorkerSocket.ClientId);
				}
				OnClientConnect?.Invoke(newClientId);
				WaitForData(newClientId);
				MainSocket.BeginAccept(new AsyncCallback(OnReceiveConnection), null);
			}
			catch (ObjectDisposedException) {
				Logger.Error("Can't finalize client connection: Socket was closed");
			}
			catch (SocketException se) {
				Logger.Error("Socket exception on attempt receive connection, " + se?.Message);
				//OnClientDisconnect?.Invoke()
			}
		}

		/// <summary>
		/// async data waiting
		/// </summary>
		/// <param name="clientId"></param>
		private void WaitForData(string clientId) {
			if (!WorkerSockets.ContainsKey(clientId)) {
				return;
			}

			try {
				TransferPackage package = new TransferPackage(WorkerSockets[clientId].Socket, clientId);
				WorkerSockets[clientId].Socket.BeginReceive(package.Buffer, 0, package.Buffer.Length, SocketFlags.None, new AsyncCallback(DataReceiveComplete), package);

			}
			catch (SocketException se) {
				try {
					OnClientDisconnect?.Invoke(clientId);
					Logger.Error("Server Socket Exception on data waiting: " + se?.Message);
				}
				catch { }
			}
			catch (Exception e) {
				OnClientDisconnect?.Invoke(clientId);
				string message = e.InnerException == null ? e.Message : e.InnerException.Message;
				Logger.Error("Service exception on data waiting: " + message);
			}

		}

		/// <summary>
		/// callback to complete data receiving from client
		/// </summary>
		/// <param name="res"></param>
		private void DataReceiveComplete(IAsyncResult res) {
			TransferPackage package = res.AsyncState as TransferPackage;
			string clientId = package.ClientId;

			try {
				int dataSize = package.CurrentSocket.EndReceive(res);
				// data size = 0 => client lost packages, stop connection
				if (dataSize == 0) {
					// TO DO 
					++WorkerSockets[clientId].EmptyDataReceived;
					if (WorkerSockets[clientId].EmptyDataReceived >= ClientMaxBadPackets) {
						OnClientDisconnect?.Invoke(clientId);
						return;
					}
				} else {
					WorkerSockets[clientId].EmptyDataReceived = 0;
				}
				OnClientReceiveData?.Invoke(clientId, package.Buffer, dataSize);
				//Logger.Log("data: " + Encoding.UTF8.GetString(package.Buffer) + " : end data");
				WaitForData(clientId);
			}
			catch (ObjectDisposedException) {
				Logger.Error("OnDataReceived: Socket has been closed");

				OnClientDisconnect?.Invoke(clientId);
			}
			catch (SocketException se) {
				//10060 - A connection attempt failed because the connected party did not properly respond after a period of time,
				//or established connection failed because connected host has failed to respond.
				if (se.ErrorCode == 10054 || se.ErrorCode == 10060) { //10054 - Error code for Connection reset by peer
					try {
						if (se.ErrorCode != 10054) {
							Logger.Error("Server exception in OnClientDataReceived, ServerObject removed:(" + se.ErrorCode.ToString() + ") " + clientId);
							Logger.Error("RemoteEndPoint: " + WorkerSockets[clientId]?.Socket?.RemoteEndPoint.ToString());
							Logger.Error("LocalEndPoint: " + WorkerSockets[clientId]?.Socket?.LocalEndPoint.ToString());
						}
					}
					catch {

					}

					OnClientDisconnect?.Invoke(clientId);

					//Logger.Log("Closing socket from OnDataReceived");
				} else {
					string mess = "connection booted for reason other than 10054: code = " + se.ErrorCode.ToString() + ",   " + se.Message;
					Logger.Error(mess);
				}
			}
		}

		public void SendData(string clientId, byte[] message) {
			if (WorkerSockets.ContainsKey(clientId)) {
				try {
					WorkerSockets[clientId].Socket.Send(message);
				}
				catch (SocketException se) {
					Logger.Error(se?.Message);
				}
			} else {
				Logger.Warn("Client " + clientId + " not exists");
			}
		}

	}
}
