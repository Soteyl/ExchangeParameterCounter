using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeParameterCounterClient
{
    [Serializable]
    public class Delimiters: IGettable
    {
        public string BetweenPackages = " ";
        public string BetweenNumberAndValue = "_";

        public void UpdateFromGetter(object obj)
        {
            if (obj is Delimiters delims)
            {
                BetweenPackages = delims.BetweenPackages;
                BetweenNumberAndValue = delims.BetweenNumberAndValue;
            }
        }
    }
}
