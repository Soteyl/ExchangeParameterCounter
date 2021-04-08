using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ExchangeParameterCounterClient
{
    public class Client: ClientConfig, IMessageReceiver
    {
        private IPAddress _ip;
        private IPEndPoint ipEndPoint;
        public Client(ClientConfig config): base(config.MulticastIP, config.MulticastPort, config.TTL, config.ReceptionDelayInMiliseconds)
        {
            _ip = IPAddress.Parse(MulticastIP);
        }

        public void ReceiveMessageWithCallBack(IDataProcess process)
        {
            while (true)
            {
                try
                {
                    ipEndPoint = null;
                    using (UdpClient udpClient = new UdpClient(MulticastPort))
                    {
                        udpClient.JoinMulticastGroup(_ip, TTL);

                        while (true)
                        {
                            Thread.Sleep(ReceptionDelayInMiliseconds);

                            List<byte> data = new List<byte>();
                            while (udpClient.Available > 0)
                            {
                                data.AddRange(udpClient.Receive(ref ipEndPoint));
                            }
                            new Thread(() => process.OnReceivingData(data)).Start();
                        }

                    }
                }
                
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Thread.Sleep(5000);
                }
            }
        }
    }
}
