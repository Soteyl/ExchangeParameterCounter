using System;
using System.IO;
using System.Xml.Serialization;
using ExchangeParameterCounterServer;

namespace SerializationXML
{
	public static class Deserializer
	{
		public static ServerConfig GetServerConfig()
		{
			XmlSerializer formatter = new XmlSerializer(typeof(ServerConfig));
			using (FileStream fs = new FileStream("server.config", FileMode.OpenOrCreate))
			{
				ServerConfig clientConfig = (ServerConfig)formatter.Deserialize(fs);
				return clientConfig;
			}
		}
	}
}
