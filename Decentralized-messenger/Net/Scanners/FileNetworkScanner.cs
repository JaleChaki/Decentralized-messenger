using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Messenger.Net.Scanners {
	public class FileNetworkScanner : INetworkScanner {
		
		public IEnumerable<IPAddress> Scan() {
			string[] ips = File.ReadAllLines("ips.txt");
			foreach (var ip in ips) {
				yield return IPAddress.Parse(ip);
			}
		}
	}
}
