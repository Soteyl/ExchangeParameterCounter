using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ExchangeParameterCounterClient
{
    public class XmlSaver : ISaver
    {
        public void Save<T>(T obj, string path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path))) Directory.CreateDirectory(Path.GetDirectoryName(path));
            XmlSerializer formatter = new XmlSerializer(typeof(T));
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                formatter.Serialize(fs, obj);
                fs.Close();
            }
        }
    }
}
