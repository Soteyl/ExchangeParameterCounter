using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeParameterCounterClient
{
    public interface IGettable
    {
        void UpdateFromGetter(object obj);
    }
}
