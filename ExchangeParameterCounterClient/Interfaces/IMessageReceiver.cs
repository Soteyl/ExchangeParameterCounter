
namespace ExchangeParameterCounterClient
{
    public interface IMessageReceiver
    {
        void ReceiveMessageWithCallBack(IDataProcess process);
        int ReceptionDelayInMiliseconds { get; set; }
    }
}
