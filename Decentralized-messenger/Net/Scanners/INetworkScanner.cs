using System.Collections.Generic;
using System.Net;

namespace Messenger.Net.Scanners {
	public interface INetworkScanner {

		IEnumerable<IPAddress> Scan();

	}
}
