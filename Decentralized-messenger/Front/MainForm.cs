using Messenger.Config;
using Messenger.Net;
using Messenger.Net.Events;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;
using XLogger;
using XLogger.Configuration;

namespace Messenger.Front {
	public partial class MainForm : Form {

		private INetworkController NetworkController;

		private IMessageContainerFactory MessageContainerFactory;

		private IClientButtonFactory ClientButtonFactory;

		private string SelectedUserId = "YYY";

		public MainForm() {
			LoggerConfiguration.ConfigureLoggerConfiguration(
				configBuilder => configBuilder
					.UseFileLogging("log.txt")
					.UseConsoleLogging()
					.UseLogLevel(LogLevel.Debug)
			);
			InitializeComponent();
			MessageContainerFactory = new DefaultMessageContainerFactory();
			ClientButtonFactory = new DefaultClientButtonFactory();
			NetworkController = new DefaultNetworkController(JsonConvert.DeserializeObject<NetworkConfig>(File.ReadAllText("Network.json")));
			NetworkController.Start();
		}

		private void MessageTextBox_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter) {
				NetworkController.SendMessage(MessageTextBox.Text, SelectedUserId);
				MessageTextBox.Text = "";
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		private void NetworkUpdateTimer_Tick(object sender, System.EventArgs args) {
			NetworkController.Update();
			var events = NetworkController.GetUpdates();
			foreach (var e in events) {
				MessageReceivedEvent msg = e as MessageReceivedEvent;
				Logger.Debug("message " + msg.FromId + " " + msg.MessageContent);
				Control c = MessageContainerFactory.CreateText(msg.FromId, msg.MessageContent);
				DialogueContainer.Controls.Add(c);
			}
			ClientsContainer.Controls.Clear();
			foreach (var c in NetworkController.OnlineClients) {
				ClientsContainer.Controls.Add(ClientButtonFactory.Create(c, new EventHandler(ClientButtonClick)));
			}
		}

		private void ClientButtonClick(object sender, EventArgs args) {
			SelectedUserId = (sender as Control).Tag.ToString();
		}

	}
}
