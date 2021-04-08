using System;
namespace ExchangeParameterCounterClient
{
    [Serializable]
    public class PathInfo: IGettable
    {
        private static PathInfo instance;

        public PathInfo() { }

        public static PathInfo GetInstance()
        {
            if (instance == null)
            {
                instance = new PathInfo();
                new XmlGetter("path.info").GetData(instance);
            }
            return instance;
        }
        public string ClientConfigPath { get; set; } // => "client.config";
        public string DataPath { get; set; } // => "data";
        public string DelimitersPath { get; set; }
        public string PackagesFilePath { get; set; } //=> DataPath + "/packages.txt";
        public string DataInfo { get; set; } //=> DataPath + "/data.info";
        public string SaveToFileConfig { get; set; }

        public void UpdateFromGetter(object obj)
        {
            if (obj is PathInfo inf)
            {
                ClientConfigPath = inf.ClientConfigPath;
                DataPath = inf.DataPath;
                PackagesFilePath = inf.PackagesFilePath;
                DataInfo = inf.DataInfo;
                DelimitersPath = inf.DelimitersPath;
                SaveToFileConfig = inf.SaveToFileConfig;
            }
        }
    }
}
