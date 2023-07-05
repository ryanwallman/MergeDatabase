using MergeDatabase;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

public class Program
{
    public static void Main()
    {
        string databaseFilePath = "database.json";
        File.WriteAllText(databaseFilePath, "[]");  // Create an empty JSON array

        AbuseIpMerger merger = new AbuseIpMerger(databaseFilePath);
        List<AbuseIpDbModel> abuseIps;

        string userInput;
        bool hasMoreInputs = true;

        while (hasMoreInputs)
        {
            Console.Write("Enter a Data Source (or 'q' to quit): ");
            userInput = Console.ReadLine();

            if (userInput.ToLower() == "q")
            {
                hasMoreInputs = false;
            }
            else
            {
                string connectionString = $"Data Source={userInput};Initial Catalog=Bubbles;User ID=sa;Password=Cyber@123;";

                try
                {
                    AbuseIpRepository repository = new AbuseIpRepository(connectionString);
                    abuseIps = repository.GetAbuseIps().DistinctBy(x => x.ipAddress).ToList();

                    merger.MergeDatabaseWithJson(ConvertToDatabaseModelList(abuseIps));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while retrieving the abuse IPs:");
                    Console.WriteLine(ex.Message);
                    // Handle the exception or perform any necessary cleanup
                }
            }
        }

        // Get the final merged JSON from the merger object
        string finalJson = merger.GetMergedJson();

        // Save the final merged JSON to a file
        string outputPath = "output.json";
        File.WriteAllText(outputPath, finalJson);
        Console.WriteLine($"Final merged database saved to: {outputPath}");
    }


    // Convert AbuseIpDbModel list to DatabaseModel list
    static List<AbuseIpDbModel> ConvertToDatabaseModelList(List<AbuseIpDbModel> abuseIps)
    {
        List<AbuseIpDbModel> databaseModels = new List<AbuseIpDbModel>();

        foreach (AbuseIpDbModel abuseIp in abuseIps)
        {
            AbuseIpDbModel databaseModel = new AbuseIpDbModel
            {
                Id = Guid.NewGuid(),
                ipAddress = abuseIp.ipAddress,
                isPublic = abuseIp.isPublic,
                ipVersion = abuseIp.ipVersion,
                isWhitelisted = abuseIp.isWhitelisted,
                abuseConfidenceScore = abuseIp.abuseConfidenceScore,
                usageType = abuseIp.usageType,
                isp = abuseIp.isp,
                domain = abuseIp.domain,
                totalReports = abuseIp.totalReports,
                numDistinctUsers = abuseIp.numDistinctUsers,
                country = abuseIp.country,
                countryCode = abuseIp.countryCode,
                region = abuseIp.region,
                regionName = abuseIp.regionName,
                city = abuseIp.city,
                org = abuseIp.org,
                @as = abuseIp.@as
            };


            databaseModels.Add(databaseModel);
        }
        return databaseModels;
    }

    public class AbuseIpRepository
    {
        private readonly string connectionString;

        public AbuseIpRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<AbuseIpDbModel> GetAbuseIps()
        {
            List<AbuseIpDbModel> abuseIps = new List<AbuseIpDbModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT [id], [ipAddress], [isPublic], [ipVersion], [isWhitelisted], [abuseConfidenceScore], [usageType], [isp], [domain], [totalReports], [numDistinctUsers], [as], [city], [country], [countryCode], [org], [region], [regionName] FROM [Bubbles].[dbo].[AbuseIpsDb]";

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    AbuseIpDbModel abuseIp = new AbuseIpDbModel();
                    abuseIp.Id = reader.IsDBNull(0) ? Guid.Empty : reader.GetGuid(0);
                    abuseIp.ipAddress = reader.IsDBNull(1) ? null : reader.GetString(1);
                    abuseIp.isPublic = reader.IsDBNull(2) ? null : (bool?)reader.GetBoolean(2);
                    abuseIp.ipVersion = reader.IsDBNull(3) ? null : (int?)reader.GetInt32(3);
                    abuseIp.isWhitelisted = reader.IsDBNull(4) ? false : reader.GetBoolean(4);
                    abuseIp.abuseConfidenceScore = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                    abuseIp.usageType = reader.IsDBNull(6) ? null : reader.GetString(6);
                    abuseIp.isp = reader.IsDBNull(7) ? null : reader.GetString(7);
                    abuseIp.domain = reader.IsDBNull(8) ? null : reader.GetString(8);
                    abuseIp.totalReports = reader.IsDBNull(9) ? 0 : reader.GetInt32(9);
                    abuseIp.numDistinctUsers = reader.IsDBNull(10) ? 0 : reader.GetInt32(10);
                    abuseIp.@as = reader.IsDBNull(11) ? null : reader.GetString(11);
                    abuseIp.city = reader.IsDBNull(12) ? null : reader.GetString(12);
                    abuseIp.country = reader.IsDBNull(13) ? null : reader.GetString(13);
                    abuseIp.countryCode = reader.IsDBNull(14) ? null : reader.GetString(14);
                    abuseIp.org = reader.IsDBNull(15) ? null : reader.GetString(15);
                    abuseIp.region = reader.IsDBNull(16) ? null : reader.GetString(16);
                    abuseIp.regionName = reader.IsDBNull(17) ? null : reader.GetString(17);

                    abuseIps.Add(abuseIp);
                }

                reader.Close();
            }
            return abuseIps;
        }
    }
}
