using System;
using System.IO;
using System.Threading;

namespace ExchangeParameterCounterClient
{
    class Program
    {
        static void Main(string[] args)
        {
            IDataGetter xmlGetter = new XmlGetter();
            IShower consoleShower = new ConsoleShower();
            ISaver xmlSaver = new XmlSaver();
            ISaver dataTxtSaver = new DataTxtSaver();
            PathInfo path = GetDataToNewObj<PathInfo>("path.info", xmlGetter);

            CreateDirectoriesIfNotExists(path);
            Delimiters delimiters = new Delimiters();
            ClientConfig clientConfig = GetDataToNewObj<ClientConfig>(path.ClientConfigPath, xmlGetter);
            SaveToFileConfig saveToFileConfig = GetDataToNewObj<SaveToFileConfig>(path.SaveToFileConfig, xmlGetter);


            Client client = new Client(clientConfig);
            DataInfo dataInfo = new DataInfo(xmlGetter,consoleShower, xmlSaver, dataTxtSaver, client,path,delimiters);

            dataInfo.BeginReceivingData(saveToFileConfig.NeedsSaveDataToFile);

            OnEnterClick(dataInfo);

        }

        private static T GetDataToNewObj<T>(string path, IDataGetter getter) where T: IGettable, new()
        {
            T obj = new T();
            getter.GetData(obj, path);
            return obj;
        }

        private static void CreateDirectoriesIfNotExists(PathInfo path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path.DataInfo))) Directory.CreateDirectory(Path.GetDirectoryName(path.DataInfo));
        }

        private static void OnEnterClick(DataInfo dataInfo)
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
