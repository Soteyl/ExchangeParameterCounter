using System;
using System.IO;
using System.Xml.Serialization;
using ExchangeParameterCounterClient;

namespace SerializationXML
{
	public static class Serializer
	{
		public static T GetDataFromXml<T>(string path)where  T: new()
        {
			XmlSerializer formatter = new XmlSerializer(typeof(T));
			using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
			{
				try
				{
					T data = (T)formatter.Deserialize(fs);
					return data;
				}
				catch(InvalidOperationException ex)
				{
					return new T();
				}
			}
		}
		public static void ChangeDataToXml<T>(T obj, string path)
        {
			XmlSerializer formatter = new XmlSerializer(typeof(T));
			using (FileStream fs = new FileStream(path, FileMode.Create))
            {
				formatter.Serialize(fs, obj);
            }
        }
	}
}

