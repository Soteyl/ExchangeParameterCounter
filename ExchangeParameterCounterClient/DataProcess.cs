using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SerializationXML;

namespace ExchangeParameterCounterClient
{
    public static class DataProcess
    {
        public static DataInfo Info;
        public static int MaxSizeOfFile = 0;
        static DataProcess()
        {
            Info = Serializer.GetDataFromXml<DataInfo>(PathInfo.DataInfo);
        }
        /// <summary>
        /// Gets all packages with their numbers
        /// </summary>
        /// <returns>string array like "number-value"</returns>
        public static string[] GetAllLastPackagesWithNumbers()
        {
            string[] value = Encoding.Default.GetString(GetBytesFromFile($"{PathInfo.DataPath}/data{Info.LastDataFileNumber}.txt")).Split(' ');
            Array.Resize(ref value, value.Length - 1); // after split last element is always empty
            return value;
        }
        /// <summary>
        /// Gets only values
        /// </summary>
        /// <param name="allPackagesWithNumbers"></param>
        /// <returns>array of int values</returns>
        public static int[] GetAllLastPackages(string[] allPackagesWithNumbers)
        {
            int[] result = new int[allPackagesWithNumbers.Length];
            for(int i = 0; i < allPackagesWithNumbers.Length; i++)
            {
                result[i] = int.Parse(allPackagesWithNumbers[i].Split('_')[1]);
            }
            return result;
        }

        public static void SaveBytesToAFile(List<byte> bytes)
        {
            string path = $"{PathInfo.DataPath}/data{Info.LastDataFileNumber}.txt";
            
            using (FileStream fs = new FileStream(path, FileMode.Append))
            {
                fs.Write(bytes.ToArray());
            }
            CreateNewFileIfCurrentIsBig(path);
        }

        private static void CreateNewFileIfCurrentIsBig(string path)
        {
            FileInfo dataFile = new FileInfo(path);
            
            if (dataFile.Length > MaxSizeOfFile)
            {
                var lastData = GetAllDataInfo();
                Info.AddNewValuesToData(lastData);
                Info.LastDataFileNumber++;
                Info.SaveToXml();
            }

        }
        public static byte[] GetBytesFromFile(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                byte[] array = new byte[fs.Length];
                fs.Read(array, 0, array.Length);
                return array;
            }
        }

        public static DataInfo GetAllDataInfo()
        {
            string[] allPackagesWithNumbers = GetAllLastPackagesWithNumbers();
            int[] allPackages = GetAllLastPackages(allPackagesWithNumbers);

            return new DataInfo(CountLostPackages(allPackagesWithNumbers),
                                CountAverage(allPackages),
                                CountStandartDeviation(allPackages),
                                CountModes(allPackages),
                                CountMedian(allPackages),
                                CountRepeats(allPackages),
                                allPackages.Length);

        }

        public static int CountLostPackages(string[] allPackagesWithNumbers)
        {
            int lostAmount = -1;
            int numberOfPackage = 0;
            int lastNumber = 0;
            for (int i = 1; i < allPackagesWithNumbers.Length; i++)
            {
                lastNumber = numberOfPackage;
                if (int.TryParse(allPackagesWithNumbers[i].Split('_')[0], out numberOfPackage)) // package looks like *number*-*value*
                {
                    if (numberOfPackage - lastNumber != 1 && (numberOfPackage != 0 && lastNumber != 999)) // the package is numbered from 0 to 999
                    {
                        if (lastNumber > numberOfPackage) lostAmount += numberOfPackage + (999 - lastNumber);
                        else lostAmount += numberOfPackage-lastNumber;
                    }
                }
            }
            return Info.LostPackagesAmount + lostAmount;
        }
        public static float CountAverage(int[] allPackages)
        {
            int sum = 0;
            foreach(var package in allPackages)
            {
                sum += package;
            }
            return ((float)sum + Info.Average*Info.SavedPackagesAmount) / (allPackages.Length+Info.SavedPackagesAmount);
        }

        public static double CountStandartDeviation(int[] allPackages)
        {
            float average = CountAverage(allPackages);
            double sum = 0;
            foreach(var package in allPackages)
            {
                sum += Math.Pow(package-average,2);
            }
            return (Math.Sqrt(sum / (allPackages.Length - 1)) * allPackages.Length+
                    Info.StandartDeviation * Info.SavedPackagesAmount)/
                    (allPackages.Length+Info.SavedPackagesAmount);
        }

        private static int GetKeyOfMax(XmlSerializableDictionary<int, int> array)
        {
            int result = 0;
            foreach(var element in array)
            {
                if (element.Value > array[result])
                {
                    result = element.Key;
                }
            }
            return result;
        }

        public static List<int> CountModes(int[] allPackages)
        {

            XmlSerializableDictionary<int, int> repeatsOfValue = CountRepeats(allPackages);

            int mode = GetKeyOfMax(repeatsOfValue);
            
            List<int> modes = new List<int>();
            foreach (var element in repeatsOfValue)
            {
                if (element.Value == repeatsOfValue[mode])
                {
                    modes.Add(element.Key);
                }
            }
            return modes;
        }

        public static int CountMedian(int[] allPackages)
        {
            XmlSerializableDictionary<int, int> repeats = CountRepeats(allPackages);
            int sum = 0;
            int currentRepeat = 0;
            var sortedDict = new SortedDictionary<int, int>(repeats);
            foreach(var element in sortedDict)
            {
                sum += element.Value;
                currentRepeat = element.Key;
                if (sum > (Info.SavedPackagesAmount + allPackages.Length) / 2) break;
            }
            return currentRepeat;
        }
        /// <summary>
        /// Needed to count Median.
        /// </summary>
        /// <param name="allPackages"></param>
        /// <returns>Array of int values repeats.</returns>
        private static XmlSerializableDictionary<int, int> CountRepeats(int[] allPackages)
        {
            XmlSerializableDictionary<int, int> repeatsOfValue = XmlSerializableDictionary<int, int>.DeepCopy<int, int>(Info.RepeatsOfValues);
            foreach (var package in allPackages)
            {
                if (repeatsOfValue.ContainsKey(package) == false)
                {
                    repeatsOfValue.Add(package, 0);
                }
                repeatsOfValue[package]++; // count repeats
            }
            return repeatsOfValue;
        }
    }
}
