using System;
using System.IO;
using System.Xml.Serialization;

namespace ExchangeParameterCounterClient
{
    class XmlGetter : IDataGetter
    {
		private string _path;
		public XmlGetter(string path)
        {
			_path = path;
        }
        public void GetData<T>(T obj) where T : IGettable, new()
        {
			T data = default(T);
			XmlSerializer formatter = new XmlSerializer(typeof(T));
			using (FileStream fs = new FileStream(_path, FileMode.OpenOrCreate))
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
