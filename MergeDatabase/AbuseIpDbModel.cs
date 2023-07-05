using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergeDatabase
{
    public class AbuseIpDbModel
    {
        [Key]
        public Guid Id { get; set; }
        public string ipAddress { get; set; }
        public bool? isPublic { get; set; }
        public int? ipVersion { get; set; }
        public bool isWhitelisted { get; set; }
        public int abuseConfidenceScore { get; set; }
        public string? usageType { get; set; }
        public string? isp { get; set; }
        public string? domain { get; set; }
        public int totalReports { get; set; }
        public int numDistinctUsers { get; set; }
        public string country { get; set; }
        public string countryCode { get; set; }
        public string region { get; set; }
        public string regionName { get; set; }
        public string city { get; set; }
        public string org { get; set; }
        public string @as { get; set; }
    }
}
