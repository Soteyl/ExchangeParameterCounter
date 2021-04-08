using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeParameterCounterClient
{
    public interface IMessageReceiver
    {
        void ReceiveMessageWithCallBack(Action<List<byte>> action);
        int ReceptionDelayInMiliseconds { get; set; }
    }
}
