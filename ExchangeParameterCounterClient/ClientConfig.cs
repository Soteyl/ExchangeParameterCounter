using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeParameterCounterClient
{
	[Serializable]
	public class ClientConfig
	{
		public string MulticastIP { get; set; }
		public int MulticastPort { get; set; }
		public int TTL { get; set; }
		public int ReceptionDelayInMiliseconds { get; set; }

		public ClientConfig() { }
		public ClientConfig(string multicastIP, int multicastPort, int ttl, int receptionDelayInMiliseconds)
        {
			MulticastIP = multicastIP;
			MulticastPort = multicastPort;
			TTL = ttl;
			ReceptionDelayInMiliseconds = receptionDelayInMiliseconds;
        }
	}
}
