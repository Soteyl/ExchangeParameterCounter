using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeParameterCounterClient
{
    public class ConsoleShower : IShower
    {
        public void Show(object obj)
        {
            Console.WriteLine(obj.ToString());
        }
    }
}
