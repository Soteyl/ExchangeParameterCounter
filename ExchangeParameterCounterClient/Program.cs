using System;
using System.IO;

namespace ExchangeParameterCounterClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = PathInfo.GetInstance();

            CreateDirectoriesIfNotExists(path);
            
            ClientConfig clientConfig = GetDataToNewObj<ClientConfig>(path.ClientConfigPath);
            SaveToFileConfig saveToFileConfig = GetDataToNewObj<SaveToFileConfig>(path.SaveToFileConfig);
            DataInfo Info = GetDataToNewObj<DataInfo>(path.DataInfo);

            ISaver xmlSaver = new XmlSaver(path.DataInfo);
            ISaver dataTxtSaver = new DataTxtSaver(path.PackagesFilePath);
            IShower consoleShower = new ConsoleShower();

            Client client = new Client(clientConfig);
            DataProcess dataInfo = new DataProcess(Info,consoleShower, xmlSaver, dataTxtSaver, client);

            dataInfo.BeginReceivingData(saveToFileConfig.NeedsSaveDataToFile);

            OnEnterClick(dataInfo);

        }

        private static T GetDataToNewObj<T>(string path) where T: IGettable, new()
        {
            T obj = new T();
            new XmlGetter(path).GetData(obj);
            return obj;
        }

        private static void CreateDirectoriesIfNotExists(PathInfo path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path.DataInfo))) Directory.CreateDirectory(Path.GetDirectoryName(path.DataInfo));
        }

        private static void OnEnterClick(DataProcess dataInfo)
        {
            while (true)
            {
                if (Console.ReadKey().Key == ConsoleKey.Enter)
                {
                    dataInfo.Show();
                }
            }
        }

    }
}
