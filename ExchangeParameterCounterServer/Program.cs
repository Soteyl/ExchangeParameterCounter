using System;
using System.Threading;
using SerializationXML;

namespace ExchangeParameterCounterServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerConfig serverConfig = Deserializer.GetServerConfig();
            Server server = new Server(serverConfig);
            Thread threadStart = new Thread(server.Start);
            threadStart.Start();
            while(true)
            {
                Thread.Sleep(5000);
                var newConfig = Deserializer.GetServerConfig();
                if (!serverConfig.Equals(newConfig))
                {
                    Console.WriteLine("Config was updated");
                    serverConfig = newConfig;
                    server.UpdateConfig(serverConfig);
                }
            }
        }
    }
}
