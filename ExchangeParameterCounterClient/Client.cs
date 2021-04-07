using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ExchangeParameterCounterClient
{
    public class Client: ClientConfig
    {
        private IPAddress _ip;
        private Thread SaveData;
        public Client(ClientConfig config): base(config.MulticastIP, config.MulticastPort, config.TTL, config.ReceptionDelayInMiliseconds)
        {
            _ip = IPAddress.Parse(MulticastIP);
        }

        public void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    IPEndPoint ipEndPoint = null;
                    using (UdpClient udpClient = new UdpClient(MulticastPort))
                    {
                        udpClient.JoinMulticastGroup(_ip, TTL);
                        while (true) // get data and save to a file every ReceptionDelayInMiliseconds
                        {
                            Thread.Sleep(ReceptionDelayInMiliseconds);
                            List<byte> data = new List<byte>();
                            while (udpClient.Available > 0)
                            {
                                data.AddRange(udpClient.Receive(ref ipEndPoint));
                            }


                            SaveData = new Thread(() => DataProcess.SaveBytesToAFile(data));
                            SaveData.Start();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Thread.Sleep(10000);
                }
            }
        }
    }
}
