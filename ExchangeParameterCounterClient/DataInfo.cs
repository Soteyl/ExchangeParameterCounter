using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace ExchangeParameterCounterClient
{
    [Serializable]
    public class DataInfo: IGettable
    {
        public int LostPackagesAmount { get; set; }
        public int SavedPackagesAmount { get; set; }

        public XmlSerializableDictionary<int, int> RepeatsOfValues { get; set; }
        public float Average { get; set; }
        public double StandartDeviation { get; set; }
        public List<int> Modes { get; set; } = new List<int>();
        public int Median { get; set; }

        private Delimiters _delimiters;
        private PathInfo _path;
        private IDataGetter _dataInfoGetter;
        private ISaver _saverInfo;
        private ISaver _saverData;
        private IShower _shower;
        private IMessageReceiver _client;
        private bool _receivingData;
        private bool _needSaveDataToFile;
        public DataInfo()
        {
            RepeatsOfValues = new XmlSerializableDictionary<int, int>();
        }
        public DataInfo(int lostPackagesAmount,
                        float average, 
                        double standartDeviation,
                        List<int> modes, 
                        int median, 
                        XmlSerializableDictionary<int, int> repeatsOfValues,
                        int savedPackagesAmount)
        {
            LostPackagesAmount = lostPackagesAmount;
            Average = average;
            StandartDeviation = standartDeviation;
            Modes = modes;
            Median = median;
            RepeatsOfValues = repeatsOfValues;
            SavedPackagesAmount = savedPackagesAmount;
        }
        public DataInfo(IDataGetter dataInfoGetter, IShower shower, ISaver saverInfo, ISaver saverData, IMessageReceiver client, PathInfo path, Delimiters delimiters)
        {
            _dataInfoGetter = dataInfoGetter;
            _shower = shower;
            _path = path;
            _saverInfo = saverInfo;
            _saverData = saverData;
            _client = client;
            _delimiters = delimiters;
            dataInfoGetter.GetData(this, _path.DataInfo);
        }

        public void BeginReceivingData(bool needsSaveDataToFile)
        {
            _needSaveDataToFile = needsSaveDataToFile;

            new Thread(()=> _client.ReceiveMessageWithCallBack((x) => OnReceivingData(x))).Start();
        }

        private void OnReceivingData(List<byte> data)
        {
            ProcessData(data, _needSaveDataToFile);
        }
        private void ProcessData(List<byte> data, bool NeedsSaveDataToFile)
        {
            if (NeedsSaveDataToFile)
            {
                _saverData.Save(data, _path.PackagesFilePath);
            }
            var preparedData = PrepareData(data);
            ProcessNewInfo(preparedData);
        }

        private List<Package> PrepareData(List<byte> data)
        {
            List<Package> prepared = new List<Package>();
            string byteToString = Encoding.Default.GetString(data.ToArray());
            string[] splittedByPackages = byteToString.Split(_delimiters.BetweenPackages);
            foreach(var package in splittedByPackages)
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
            return prepared;
        }
        public void StopReceivingData()
        {
            _receivingData = false;
        }
        private void ProcessNewInfo(List<Package> data)
        {
            LostPackagesAmount += CountLostPackages(data);

            float allPackagesSum = (Average * SavedPackagesAmount + CountAverage(data) * data.Count);
            Average = allPackagesSum / (SavedPackagesAmount + data.Count);

            var newDeviation = CountStandartDeviation(data);
            StandartDeviation = UnitStandartDeviation(newDeviation, data.Count);

            UpdateRepeats(data);

            Modes = CountModes(data);

            SavedPackagesAmount += data.Count;

            Median = CountMedian();


            Save();
        }

        private List<int> CountModes(List<Package> data)
        {

            int mode = GetKeyOfMax(RepeatsOfValues);

            List<int> modes = new List<int>();
            foreach (var element in RepeatsOfValues)
            {
                if (element.Value == RepeatsOfValues[mode])
                {
                    modes.Add(element.Key);
                }
            }
            return modes;
        }

        private int CountMedian()
        {
            int sum = 0;
            int currentRepeat = 0;
            var sortedDict = new SortedDictionary<int, int>(RepeatsOfValues);
            foreach (var element in sortedDict)
            {
                sum += element.Value;
                currentRepeat = element.Key;
                if (sum > (SavedPackagesAmount / 2)) break;
            }
            return currentRepeat;
        }

        private int GetKeyOfMax(XmlSerializableDictionary<int, int> array)
        {
            int result; 
            var e = array.GetEnumerator();
            e.MoveNext();
            result = e.Current.Key;
            foreach (var element in array)
            {
                if (element.Value > array[result])
                {
                    result = element.Key;
                }
            }
            return result;
        }

        private void UpdateRepeats(List<Package> data)
        {
            XmlSerializableDictionary<int, int> repeatsOfValue = XmlSerializableDictionary<int, int>.DeepCopy(RepeatsOfValues);
            foreach (var package in data)
            {
                if (repeatsOfValue.ContainsKey(package.Value) == false)
                {
                    repeatsOfValue.Add(package.Value, 0);
                }
                repeatsOfValue[package.Value]++; // count repeats
            }
            RepeatsOfValues = repeatsOfValue;
        }
        private int CountLostPackages(List<Package> data)
        {
            int lostAmount = 0;
            for (int i = 1; i < data.Count; i++)
            {
                int lastNumber = data[i - 1].Number;
                int currentNumber = data[i].Number;
                if (currentNumber - lastNumber != 1 || (currentNumber == 0 && lastNumber == 999)) // the package is numbered from 0 to 999
                {
                    if (lastNumber > currentNumber) lostAmount += currentNumber + (999 - lastNumber);
                    else lostAmount += currentNumber - lastNumber;
                }
            }
            return lostAmount;
        }
        private float CountAverage(List<Package> data)
        {
            int sum = 0;
            foreach (var package in data)
            {
                sum += package.Value;
            }
            return (float)sum/data.Count;
        }
        private double CountStandartDeviation(List<Package> data)
        {
            float average = Average;
            double sum = 0;
            foreach (var package in data)
            {
                var delta = package.Value - average;
                sum += delta*delta;
            }
            return Math.Sqrt(sum / (data.Count - 1));
        }

        private double UnitStandartDeviation(double newDeviation, int dataCount)
        {
            return (SavedPackagesAmount * StandartDeviation + newDeviation * dataCount) / (SavedPackagesAmount + dataCount);
        }

        public void UpdateFromGetter(object obj)
        {
            if (obj is DataInfo newData)
            {
                LostPackagesAmount = newData.LostPackagesAmount;
                SavedPackagesAmount = newData.SavedPackagesAmount;
                Average = newData.Average;
                Modes = newData.Modes;
                Median = newData.Median;
                RepeatsOfValues = newData.RepeatsOfValues;
                StandartDeviation = newData.StandartDeviation;
            }
        }
        public void Save()
        {
            _saverInfo.Save(this, _path.DataInfo);
        }
        public void Show()
        {
            _shower.Show(this);
        }
        public override string ToString()
        {
            string value = "Lost packages amount: " + LostPackagesAmount;
            value += "\nAverage: " + Average;
            value += "\nStandart Deviation: " + StandartDeviation;
            value += "\nModes: ";
            foreach(var mode in Modes)
            {
                value += mode + " ";
            }
            value += "\nMedian: " + Median;
            return value;
        }
    }
}
