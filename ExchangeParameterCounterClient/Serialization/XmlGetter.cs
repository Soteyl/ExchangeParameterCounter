using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ExchangeParameterCounterClient
{
    class XmlGetter : IDataGetter
    {
        public void GetData<T>(T obj, string path) where T : IGettable, new()
        {
			T data = default(T);
			XmlSerializer formatter = new XmlSerializer(typeof(T));
			using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
			{
				try
				{
					data = (T)formatter.Deserialize(fs);
				}
				catch (InvalidOperationException)
				{
					data = new T();
				}
				fs.Close();
			}
			obj.UpdateFromGetter(data);
        }
    }
}
