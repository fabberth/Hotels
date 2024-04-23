using CsvHelper;
using System.Globalization;

namespace Hotels.Utilities
{
    public class CsvHelperUtil
    {
        public byte[] GenerateCsvFile<T>(IEnumerable<T> data)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamWriter streamWriter = new StreamWriter(memoryStream))
            using (CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteRecords(data);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }
    }
}
