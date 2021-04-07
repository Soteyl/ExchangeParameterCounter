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

        public int[] RepeatsOfValues { get; set; } = new int[0];

        public int MaxSizeOfFile { get; set; }
        public float Average { get; set; }
        public double StandartDeviation { get; set; }
        public List<int> Modes { get; set; }
        public int Median { get; set; }
        public DataInfo()
        {
            RepeatsOfValues = new int[0];
        }
        public DataInfo(int lostPackagesAmount, float average, double standartDeviation, List<int> modes, int median, int[] repeatsOfValues, int savedPackagesAmount)
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
            LostPackagesAmount = LostPackagesAmount;
            SavedPackagesAmount += SavedPackagesAmount;
            Average = Average;
            Modes = Modes;
            Median = Median;
            RepeatsOfValues = RepeatsOfValues;
            StandartDeviation = StandartDeviation;
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
