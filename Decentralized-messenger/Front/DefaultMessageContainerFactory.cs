using System.Windows.Forms;

namespace Messenger.Front {
	class DefaultMessageContainerFactory : IMessageContainerFactory {
		
		public Control CreateText(string from, string content) {
			GroupBox box = new GroupBox();
			box.Width = 500;
			box.Height = 50;
			box.Text = from;
			Label label = new Label();
			label.Text = content;
			box.Controls.Add(label);
			label.Top = 20;
			label.Left = 7;
			return box;
			//label.Font = new System.Drawing.Font()
		}
	}
}
