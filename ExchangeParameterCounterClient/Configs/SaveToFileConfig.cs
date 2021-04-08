using System;

namespace ExchangeParameterCounterClient
{
    [Serializable]
    public class SaveToFileConfig: IGettable
    {
        public bool NeedsSaveDataToFile { get; set; }

        public void UpdateFromGetter(object obj)
        {
           if (obj is SaveToFileConfig sav)
            {
                NeedsSaveDataToFile = sav.NeedsSaveDataToFile;
            }
        }
    }
}
