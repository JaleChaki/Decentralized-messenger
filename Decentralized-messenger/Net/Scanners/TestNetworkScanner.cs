using System;
using System.Collections.Generic;
using System.Net;

namespace Messenger.Net.Scanners {
	public class TestNetworkScanner : INetworkScanner {

		public IEnumerable<IPAddress> Scan() {
			yield return IPAddress.Parse("127.0.0.1");
		}
	}
}
