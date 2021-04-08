using System;
using System.Collections.Generic;
using System.IO;
namespace ExchangeParameterCounterClient
{
    class DataTxtSaver : ISaver
    {
        private string _path;
        public DataTxtSaver(string path)
        {
            _path = path;
        }
        public void Save<T>(T obj)
        {
            if (obj is List<byte> data)
            {
                using (
                    Stream fs = new FileStream(_path, FileMode.Append))
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
