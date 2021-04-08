using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace ExchangeParameterCounterClient
{
    [Serializable]
    public class DataProcess: IDataProcess
    {
        public DataInfo Info;

        private PathInfo _path;
        private ISaver _saverInfo;
        private ISaver _saverData;
        private IShower _shower;
        private IMessageReceiver _client;
        private bool _needSaveDataToFile;
        public DataProcess()
        {
        }
        public DataProcess(DataInfo info, IShower shower, ISaver saverInfo, ISaver saverData, IMessageReceiver client)
        {
            _shower = shower;
            _saverInfo = saverInfo;
            _saverData = saverData;
            _client = client;
            Info = info;
            _path = PathInfo.GetInstance();
        }

        public void BeginReceivingData(bool needsSaveDataToFile)
        {
            _needSaveDataToFile = needsSaveDataToFile;

            new Thread(()=> _client.ReceiveMessageWithCallBack(this)).Start();
        }

        public void OnReceivingData(List<byte> data)
        {
            if (_needSaveDataToFile)
            {
                _saverData.Save(data);
            }
            var preparedData = new Data(data);
            Info.ProcessNewInfo(preparedData);
        }

        public void Save()
        {
            _saverInfo.Save(Info);
        }
        public void Show()
        {
            _shower.Show(Info);
        }
        public override string ToString()
        {
            return Info.ToString();
        }
    }
}
