using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeParameterCounterClient
{
    public class Data
    {
        public List<Package> Prepared { get; private set; }

        private Delimiters _delimiters;
        public Data(List<byte> data)
        {
            _delimiters = new Delimiters();
            Prepare(data);
        }

        private void Prepare(List<byte> data)
        {
            List<Package> prepared = new List<Package>();
            string byteToString = Encoding.Default.GetString(data.ToArray());
            string[] splittedByPackages = byteToString.Split(_delimiters.BetweenPackages);
            foreach (var package in splittedByPackages)
            {
                string[] splittedByNumberAndValue = package.Split(_delimiters.BetweenNumberAndValue);
                if (splittedByNumberAndValue[0].Length > 0)
                {
                    var preparedPackage = new Package();
                    bool parsedNumber = int.TryParse(splittedByNumberAndValue[0], out preparedPackage.Number);
                    bool parsedValue = int.TryParse(splittedByNumberAndValue[1], out preparedPackage.Value);
                    if (parsedNumber && parsedValue)
                    {
                        prepared.Add(preparedPackage);
                    }
                }
            }
            Prepared = prepared;
        }
    }
}
