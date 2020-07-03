using Messenger.Net;
using System.Windows.Forms;

namespace Messenger.Front {
	public partial class MainForm : Form {

		private INetworkController NetworkController;

		private string SelectedUserId;

		public MainForm() {
			InitializeComponent();
			
		}

		private void MessageTextBox_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter) {
				NetworkController.SendMessage(MessageTextBox.Text, SelectedUserId);
				MessageTextBox.Text = "";
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}
	}
}
