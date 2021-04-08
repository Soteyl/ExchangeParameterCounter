using System;

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
