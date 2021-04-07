using System;
using System.Threading;
using SerializationXML;

namespace ExchangeParameterCounterClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientConfig clientConfig = Serializer.GetDataFromXml<ClientConfig>(PathInfo.ClientConfigPath);
            var client = new Client(clientConfig);

            Thread threadReceive = new Thread(client.ReceiveMessage);
            threadReceive.Start();

            while (true)
            {
                if (Console.ReadKey().Key == ConsoleKey.Enter)
                {
                    Thread threadWriteConsole = new Thread(WriteInfoToConsole);
                    threadWriteConsole.Start();
                }
            }
        }

        static void WriteInfoToConsole()
        {
            Console.WriteLine(DataProcess.GetAllDataInfo());
        }
    }
}
