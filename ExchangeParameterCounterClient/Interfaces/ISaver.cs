using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeParameterCounterClient
{
    public interface ISaver
    {
        void Save<T>(T obj);
    }
}
