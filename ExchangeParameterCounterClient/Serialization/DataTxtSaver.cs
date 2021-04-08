using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ExchangeParameterCounterClient
{
    class DataTxtSaver : ISaver
    {
        public void Save<T>(T obj, string path)
        {
            if (obj is List<byte> data)
            {
                using (
                    Stream fs = new FileStream(path, FileMode.Append))
                {
                    fs.Write(data.ToArray());
                }
            }
            else
            {
                throw new Exception("type of data must be List<byte>");
            }
        }
    }
}
