using System;
using System.Windows.Forms;

namespace Messenger.Front {
	interface IClientButtonFactory {

		Control Create(string name, EventHandler onClick);

	}
}
