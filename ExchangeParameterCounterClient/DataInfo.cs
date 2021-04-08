using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeParameterCounterClient
{
    [Serializable]
    public class DataInfo : IGettable
    {
        public int LostPackagesAmount { get; set; }
        public int SavedPackagesAmount { get; set; }
        public XmlSerializableDictionary<int, int> RepeatsOfValues { get; set; } = new XmlSerializableDictionary<int, int>();
        public float Average { get; set; }
        public double StandartDeviation { get; set; }
        public List<int> Modes { get; set; } = new List<int>();
        public int Median { get; set; }

        public void ProcessNewInfo(Data data)
        {
            LostPackagesAmount += CountLostPackages(data);

            float allPackagesSum = (Average * SavedPackagesAmount + CountAverage(data) * data.Prepared.Count);
            Average = allPackagesSum / (SavedPackagesAmount + data.Prepared.Count);

            var newDeviation = CountStandartDeviation(data);
            StandartDeviation = UnitStandartDeviation(newDeviation, data.Prepared.Count);

            UpdateRepeats(data);

            Modes = CountModes();

            SavedPackagesAmount += data.Prepared.Count;

            Median = CountMedian();
        }
        private double UnitStandartDeviation(double newDeviation, int dataCount)
        {
            return (SavedPackagesAmount * StandartDeviation + newDeviation * dataCount) / (SavedPackagesAmount + dataCount);
        }

        private float CountAverage(Data data)
        {
            int sum = 0;
            foreach (var package in data.Prepared)
            {
                sum += package.Value;
            }
            return (float)sum / data.Prepared.Count;
        }
        private double CountStandartDeviation(Data data)
        {
            float average = Average;
            double sum = 0;
            foreach (var package in data.Prepared)
            {
                var delta = package.Value - average;
                sum += delta * delta;
            }
            return Math.Sqrt(sum / (data.Prepared.Count - 1));
        }

        private int CountLostPackages(Data data)
        {
            int lostAmount = 0;
            for (int i = 1; i < data.Prepared.Count; i++)
            {
                int lastNumber = data.Prepared[i - 1].Number;
                int currentNumber = data.Prepared[i].Number;
                if (currentNumber - lastNumber != 1 || (currentNumber == 0 && lastNumber == 999)) // the package is numbered from 0 to 999
                {
                    if (lastNumber > currentNumber) lostAmount += currentNumber + (999 - lastNumber);
                    else lostAmount += currentNumber - lastNumber;
                }
            }
            return lostAmount;
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

        private void UpdateRepeats(Data data)
        {
            XmlSerializableDictionary<int, int> repeatsOfValue = XmlSerializableDictionary<int, int>.DeepCopy(RepeatsOfValues);
            foreach (var package in data.Prepared)
            {
                if (repeatsOfValue.ContainsKey(package.Value) == false)
                {
                    repeatsOfValue.Add(package.Value, 0);
                }
                repeatsOfValue[package.Value]++; // count repeats
            }
            RepeatsOfValues = repeatsOfValue;
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

        private List<int> CountModes()
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
        public override string ToString()
        {
            string value = "Lost packages amount: " + LostPackagesAmount;
            value += "\nAverage: " + Average;
            value += "\nStandart Deviation: " + StandartDeviation;
            value += "\nModes: ";
            foreach (var mode in Modes)
            {
                value += mode + " ";
            }
            value += "\nMedian: " + Median;
            return value;
        }
    }
}
