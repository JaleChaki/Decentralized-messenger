using System.Windows.Forms;

namespace Messenger.Front {
	interface IMessageContainerFactory {

		Control CreateText(string from, string content);

	}
}
