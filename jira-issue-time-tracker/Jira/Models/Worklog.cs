using System.Text.Json.Serialization;

namespace jira_issue_time_tracker.Jira.Models
{
    internal class Worklog()
    {
        [JsonPropertyName("timeSpentSeconds")]
        public int TimeSpentSeconds { get; set; }
    }
}
