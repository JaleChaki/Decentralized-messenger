using Messenger.Config;
using Messenger.Messages;
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

		private IMessageSaver Saver;

		private string SelectedUserId = "";

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
			Saver = new TxtMessageSaver();
			NetworkController = new DefaultNetworkController(JsonConvert.DeserializeObject<NetworkConfig>(File.ReadAllText("Network.json")));
			NetworkController.Start();

			foreach (var t in Saver.LoadMessages()) {
				CreateMessage(t.Item1, t.Item2);
			}
		}

		private void MessageTextBox_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter) {
				NetworkController.SendMessage(MessageTextBox.Text, SelectedUserId);
				MessageTextBox.Text = "";
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		private void NetworkUpdateTimer_Tick(object sender, EventArgs args) {
			NetworkController.Update();
			var events = NetworkController.GetUpdates();
			foreach (var e in events) {
				MessageReceivedEvent msg = e as MessageReceivedEvent;
				Logger.Debug("message " + msg.FromId + " " + msg.MessageContent);
				CreateMessage(msg.FromId, msg.MessageContent);
				Saver.SaveMessage(msg.FromId, msg.MessageContent);
			}
			ClientsContainer.Controls.Clear();
			foreach (var c in NetworkController.OnlineClients) {
				ClientsContainer.Controls.Add(ClientButtonFactory.Create(c, new EventHandler(ClientButtonClick)));
			}
		}

		private void CreateMessage(string from, string content) {
			Control c = MessageContainerFactory.CreateText(from, content);
			DialogueContainer.Controls.Add(c);
		}

		private void ClientButtonClick(object sender, EventArgs args) {
			SelectedUserId = (sender as Control).Tag.ToString();
			DeliverTo.Text = "To: " + SelectedUserId;
		}

		private void FileAttachButton_Click(object sender, EventArgs e) {
			if (OpenFileDialog.ShowDialog() == DialogResult.OK) {
				NetworkController.SendFile(OpenFileDialog.FileName, SelectedUserId);
			}
		}
	}
}
