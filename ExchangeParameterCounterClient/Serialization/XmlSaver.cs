using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ExchangeParameterCounterClient
{
    public class XmlSaver : ISaver
    {
        private string _path;
        public XmlSaver(string path)
        {
            _path = path;
        }
        public void Save<T>(T obj)
        {
            if (!Directory.Exists(Path.GetDirectoryName(_path))) Directory.CreateDirectory(Path.GetDirectoryName(_path));
            XmlSerializer formatter = new XmlSerializer(typeof(T));
            using (FileStream fs = new FileStream(_path, FileMode.Create))
            {
                formatter.Serialize(fs, obj);
                fs.Close();
            }
        }
    }
}
