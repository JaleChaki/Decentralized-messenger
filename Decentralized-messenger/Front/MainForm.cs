using Messenger.Config;
using Messenger.Net;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Forms;
using XLogger;
using XLogger.Configuration;

namespace Messenger.Front {
	public partial class MainForm : Form {

		private INetworkController NetworkController;

		private string SelectedUserId = "YYY";

		public MainForm() {
			LoggerConfiguration.ConfigureLoggerConfiguration(
				configBuilder => configBuilder
					.UseFileLogging("log.txt")
					.UseConsoleLogging()
					.UseLogLevel(LogLevel.Debug)
			);
			InitializeComponent();
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

			}
		}
	}
}
