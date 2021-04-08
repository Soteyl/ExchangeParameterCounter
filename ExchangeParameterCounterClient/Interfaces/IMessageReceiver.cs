using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeParameterCounterClient
{
    public interface IMessageReceiver
    {
        void ReceiveMessageWithCallBack(IDataProcess process);
        int ReceptionDelayInMiliseconds { get; set; }
    }
}
