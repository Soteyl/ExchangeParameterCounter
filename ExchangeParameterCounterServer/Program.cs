using System;
using SerializationXML;

namespace ExchangeParameterCounterServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerConfig serverConfig = Deserializer.GetServerConfig();
            Server server = new Server(serverConfig);
            server.Start();
        }
    }
}
