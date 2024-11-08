using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace jira_issue_time_tracker.Jira.Models
{
    internal class Worklog()
    {
        [JsonPropertyName("timeSpentSeconds")]
        public int TimeSpentSeconds { get; set; }
    }
}
