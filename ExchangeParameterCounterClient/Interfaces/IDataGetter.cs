using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeParameterCounterClient
{
    public interface IDataGetter
    {
        public void GetData<T>(T obj, string path) where T : IGettable, new();
    }
}
