using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ExchangeParameterCounterClient
{
    [Serializable]
    public class DataInfo
    {
        public int LostPackagesAmount { get; set; }
        public int SavedPackagesAmount { get; set; }
        public int LastDataFileNumber { get; set; }

        public XmlSerializableDictionary<int, int> RepeatsOfValues { get; set; }
        public float Average { get; set; }
        public double StandartDeviation { get; set; }
        public List<int> Modes { get; set; }
        public int Median { get; set; }
        public DataInfo()
        {
            RepeatsOfValues = new XmlSerializableDictionary<int, int>();
        }
        public DataInfo(int lostPackagesAmount, float average, double standartDeviation, List<int> modes, int median, XmlSerializableDictionary<int, int> repeatsOfValues, int savedPackagesAmount)
        {
            LostPackagesAmount = lostPackagesAmount;
            Average = average;
            StandartDeviation = standartDeviation;
            Modes = modes;
            Median = median;
            RepeatsOfValues = repeatsOfValues;
            SavedPackagesAmount = savedPackagesAmount;
        }
        public void AddNewValuesToData(DataInfo newData)
        {
            LostPackagesAmount = newData.LostPackagesAmount;
            SavedPackagesAmount += newData.SavedPackagesAmount;
            Average = newData.Average;
            Modes = newData.Modes;
            Median = newData.Median;
            RepeatsOfValues = newData.RepeatsOfValues;
            StandartDeviation = newData.StandartDeviation;
        }
        public void SaveToXml()
        {
            SerializationXML.Serializer.ChangeDataToXml(this, PathInfo.DataInfo);
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
