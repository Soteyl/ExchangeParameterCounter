using System;
using System.IO;
using System.Threading;
using SerializationXML;

namespace ExchangeParameterCounterClient
{
    class Program
    {
        static void Main(string[] args)
        {
            CheckForDataFolder();
            ClientConfig clientConfig = Serializer.GetDataFromXml<ClientConfig>(PathInfo.ClientConfigPath);
            var client = new Client(clientConfig);
            DataProcess.MaxSizeOfFile = clientConfig.MaxSizeOfFile;
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

        static void CheckForDataFolder()
        {
            if (!Directory.Exists(PathInfo.DataPath)) Directory.CreateDirectory(PathInfo.DataPath);
        }

        static void WriteInfoToConsole()
        {
            Console.WriteLine(DataProcess.GetAllDataInfo());
        }
    }
}
