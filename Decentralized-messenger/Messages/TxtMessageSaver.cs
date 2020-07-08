using System;
using System.Collections.Generic;
using System.IO;

namespace Messenger.Messages {
	public class TxtMessageSaver : IMessageSaver {

		public IEnumerable<Tuple<string, string>> LoadMessages() {
			if (!File.Exists("msgs.txt")) {
				yield break;
			}
			string[] lines = File.ReadAllLines("msgs.txt");
			for (int i = 0; i < lines.Length; ++i) {
				string[] splitres = lines[i].Split(';');
				yield return new Tuple<string, string>(splitres[0], splitres[1]);
			}
		}

		public void SaveMessage(string fromId, string messageContent) {
			File.AppendAllText("msgs.txt", fromId + ";" + messageContent + "\n");
		}
	}
}
