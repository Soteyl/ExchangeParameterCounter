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
                result[i] = int.Parse(allPackagesWithNumbers[i].Split('-')[1]);
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
            
            if (dataFile.Length > Info.MaxSizeOfFile)
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
            for (int i = 0; i < allPackagesWithNumbers.Length; i++)
            {
                lastNumber = numberOfPackage;
                if (int.TryParse(allPackagesWithNumbers[i].Split('-')[0], out numberOfPackage)) // package looks like *number*-*value*
                {
                    if (lostAmount == -1 && numberOfPackage == 0) lostAmount = 0; 
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

        private static int GetMaxIndex(int[] array)
        {
            int result = 0;
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] > array[result]) result = i;
            }
            return result;
        }

        public static List<int> CountModes(int[] allPackages)
        {

            int[] repeatsOfValue = CountRepeats(allPackages);


            int mode = GetMaxIndex(repeatsOfValue);
            
            List<int> modes = new List<int>();
            for (int i = 0; i < repeatsOfValue.Length; i++)
            {
                if (repeatsOfValue[i] == repeatsOfValue[mode]) modes.Add(i);
            }
            return modes;
        }

        public static int CountMedian(int[] allPackages)
        {
            int[] repeats = CountRepeats(allPackages);
            int sum = 0;
            int currentRepeat = 0;
            while (sum < (Info.SavedPackagesAmount + allPackages.Length) / 2)
            {
                sum += repeats[currentRepeat];
                currentRepeat++;
            }
            return currentRepeat;
        }
        /// <summary>
        /// Needed to count Median.
        /// </summary>
        /// <param name="allPackages"></param>
        /// <returns>Array of int values repeats.</returns>
        private static int[] CountRepeats(int[] allPackages)
        {
            int[] repeatsOfValue = new int[Info.RepeatsOfValues.Length];
            Array.Copy(Info.RepeatsOfValues, repeatsOfValue, Info.RepeatsOfValues.Length);
            foreach (var package in allPackages)
            {
                if (repeatsOfValue.Length <= package)
                {
                    Array.Resize(ref repeatsOfValue, package+1);
                }
                repeatsOfValue[package]++; // count repeats
            }
            return repeatsOfValue;
        }
    }
}
