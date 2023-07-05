using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MergeDatabase
{
    public class AbuseIpMerger
    {
        private readonly string databaseFilePath;
        private List<AbuseIpDbModel> mergedDatabase;

        public AbuseIpMerger(string databaseFilePath)
        {
            this.databaseFilePath = databaseFilePath;
            mergedDatabase = LoadDatabaseFromJson();
        }

        public void MergeDatabaseWithJson(List<AbuseIpDbModel> newDatabase)
        {
            // Create a HashSet to store unique combinations of ipAddress and abuseConfidenceScore
            HashSet<(string, int)> uniqueCombinations = new HashSet<(string, int)>();

            // Create a list to store the merged database
            List<AbuseIpDbModel> mergedData = new List<AbuseIpDbModel>();

            // Iterate through the existing merged database
            foreach (var ip in mergedDatabase)
            {
                // Check if the combination of ipAddress and abuseConfidenceScore is unique
                var combination = (ip.ipAddress, ip.abuseConfidenceScore);
                if (!uniqueCombinations.Contains(combination))
                {
                    // Add the combination to the unique combinations set
                    uniqueCombinations.Add(combination);

                    // Add the entry to the merged data list
                    mergedData.Add(ip);
                }
            }

            // Iterate through the new database and merge with the existing data
            foreach (var ip in newDatabase)
            {
                // Check if the combination of ipAddress and abuseConfidenceScore is unique
                var combination = (ip.ipAddress, ip.abuseConfidenceScore);
                if (!uniqueCombinations.Contains(combination))
                {
                    // Add the combination to the unique combinations set
                    uniqueCombinations.Add(combination);

                    // Add the entry to the merged data list
                    mergedData.Add(ip);
                }
            }

            // Update the merged database with the merged data list
            mergedDatabase = mergedData;

            // Save the merged database to JSON file
            SaveDatabaseToJson();
        }

        public string GetMergedJson()
        {
            // Serialize the merged database to JSON string
            string mergedJson = JsonConvert.SerializeObject(mergedDatabase, Newtonsoft.Json.Formatting.Indented);
            return mergedJson;
        }

        private List<AbuseIpDbModel> LoadDatabaseFromJson()
        {
            if (File.Exists(databaseFilePath))
            {
                // Read the existing JSON file and deserialize it to a list of AbuseIpDbModel objects
                string jsonContent = File.ReadAllText(databaseFilePath);
                var database = JsonConvert.DeserializeObject<List<AbuseIpDbModel>>(jsonContent);
                return database ?? new List<AbuseIpDbModel>();
            }
            else
            {
                return new List<AbuseIpDbModel>();
            }
        }

        private void SaveDatabaseToJson()
        {
            // Serialize the merged database to JSON string
            string mergedJson = JsonConvert.SerializeObject(mergedDatabase, Newtonsoft.Json.Formatting.Indented);

            // Save the JSON string to the database file
            File.WriteAllText(databaseFilePath, mergedJson);
        }
    }
}

