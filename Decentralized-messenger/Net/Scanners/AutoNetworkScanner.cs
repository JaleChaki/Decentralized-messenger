using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Messenger.Net.Scanners {
	class AutoNetworkScanner : INetworkScanner {
		public IEnumerable<IPAddress> Scan() {
			return Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
		}
	}
}
