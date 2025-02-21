using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace DotNetSelenium.Utilities
{
    public class TestDataReader
    {
        public static JObject LoadJson(string fileName)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory; // Get the base directory
            string filePath = Path.Combine(basePath, "TestData", fileName); // Construct dynamic path

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Test data file not found at {filePath}");
            }

            string jsonData = File.ReadAllText(filePath);
            return JObject.Parse(jsonData);
        }
    }

}