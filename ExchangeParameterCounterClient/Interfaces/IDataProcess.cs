using System.Collections.Generic;

namespace ExchangeParameterCounterClient
{
    public interface IDataProcess
    {
        void OnReceivingData(List<byte> data);
    }
}