using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeParameterCounterClient
{
    public static class PathInfo
    {
        public static string ClientConfigPath => "client.config";
        public static string DataPath => "data";
        public static string DataInfo => DataPath + "/data.info";
    }
}
