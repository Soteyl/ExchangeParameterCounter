using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeParameterCounterServer
{

	[Serializable]
	public class ServerConfig
	{
		public string MulticastIP { get; set; }
		public int MulticastPort { get; set; }
		public int MinValue { get; set; }
		public int MaxValue { get; set; }

		public ServerConfig() { }

		public ServerConfig(string multicastIP, int multicastPort, int minValue, int maxValue)
        {
			MulticastIP = multicastIP;
			MulticastPort = multicastPort;
			MinValue = minValue;
			MaxValue = maxValue;
        }

        public override bool Equals(object obj)
        {
			if (obj is ServerConfig conf)
			{
				return (MulticastIP == conf.MulticastIP) && (MulticastPort == conf.MulticastPort) && (MinValue == conf.MinValue) && (MaxValue == conf.MaxValue);
			}
			else return false;
        }
        public override int GetHashCode()
        {
            return 13 * MulticastPort * MinValue * MaxValue + MulticastIP.GetHashCode();
        }
    }
}
