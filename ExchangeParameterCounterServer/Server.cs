using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ExchangeParameterCounterServer
{
    class Server: ServerConfig
    {
        private IPAddress _ip;
        private Random rnd;
        public Server(ServerConfig config)
        {
            rnd = new Random();
            UpdateConfig(config);
        }

        public void UpdateConfig(ServerConfig config)
        {
            MulticastIP = config.MulticastIP;
            MulticastPort = config.MulticastPort;
            MinValue = config.MinValue;
            MaxValue = config.MaxValue;
            _ip = IPAddress.Parse(MulticastIP);

        }

        public void Start()
        {
            try
            {
                var ipEndPoint = new IPEndPoint(_ip, MulticastPort);
                using (var udpClient = new UdpClient(AddressFamily.InterNetwork))
                {
                    udpClient.JoinMulticastGroup(_ip);
                    int i = 0;
                    while (true)
                    {
                        string randomValue = i + "_" + rnd.Next(MinValue, MaxValue) + " ";
                        var data = Encoding.Default.GetBytes(randomValue.ToString());
                        udpClient.Send(data, data.Length, ipEndPoint);
                        i = (i + 1) % 1000; //number packages from 0 to 999

                    }
                }
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex);
            }
        }
    }
}
