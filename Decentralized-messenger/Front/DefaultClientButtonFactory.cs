using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Messenger.Front {
	class DefaultClientButtonFactory : IClientButtonFactory {

		public Control Create(string name, EventHandler onClick) {
			Button b = new Button();
			b.Text = name;
			b.Tag = name;
			b.Width = 240;
			b.Click += onClick;
			return b;
		}
	}
}
