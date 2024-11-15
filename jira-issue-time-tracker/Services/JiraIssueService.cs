using System.Text.Json;
using jira_issue_time_tracker.Jira;
using jira_issue_time_tracker.Jira.Models;
using static jira_issue_time_tracker.Services.JiraIssueService;

namespace jira_issue_time_tracker.Services
{
    internal interface IJiraIssueService
    {
        Task UpdateWorklogAsync(string issueKey, Worklog worklog);

        Task<Issue> GetIssueAsync(string issueKey);
        Task StartTrackingIssueAsync(string issueKey, string summary);
        Task StopTrackingIssueAsync(TrackedIssue trackedIssue);

        Task<List<TrackedIssue>> GetTrackedIssuesAsync();
    }

    internal class JiraIssueService(IJiraApiClient jiraApiClient) : IJiraIssueService
    {
        public class TrackedIssue(string issueKey, string summary)
        {
            public string IssueKey { get; set; } = issueKey;
            public DateTimeOffset TrackedSince { get; set; }
            public string Summary { get; set; } = summary;

            public TimeSpan ElapsedTime => DateTimeOffset.Now - TrackedSince;
        }

        private readonly string _trackedIssuesFileLocation = Path.Combine(
            Directory.GetCurrentDirectory(),
            ".jit"
        );

        private bool DoesTrackedIssuesFileExist()
        {
            return File.Exists(_trackedIssuesFileLocation);
        }

        private async Task InitializeTrackedIssuesFileAsync()
        {
            var content = JsonSerializer.Serialize(new List<TrackedIssue>());
            await using var outputFile = new StreamWriter(_trackedIssuesFileLocation);
            await outputFile.WriteAsync(content);
        }

        public async Task<List<TrackedIssue>> GetTrackedIssuesAsync()
        {
            if (!DoesTrackedIssuesFileExist())
            {
                await InitializeTrackedIssuesFileAsync();
            }

            using StreamReader reader = new(_trackedIssuesFileLocation);
            return await JsonSerializer.DeserializeAsync<List<TrackedIssue>>(reader.BaseStream)
                ?? [];
        }

        private async Task SaveTrackedIssuesAsync(List<TrackedIssue> trackedIssues)
        {
            var content = JsonSerializer.Serialize(trackedIssues);
            await using var outputFile = new StreamWriter(_trackedIssuesFileLocation);
            await outputFile.WriteAsync(content);
        }

        public async Task<Issue> GetIssueAsync(string issueKey)
        {
            return await jiraApiClient.GetIssueAsync(issueKey);
        }

        public async Task StartTrackingIssueAsync(string issueKey, string summary)
        {
            var trackedIssues = await GetTrackedIssuesAsync();

            trackedIssues.Add(
                new TrackedIssue(issueKey, summary) { TrackedSince = DateTimeOffset.Now }
            );

            await SaveTrackedIssuesAsync(trackedIssues);
        }

        public async Task UpdateWorklogAsync(string issueKey, Jira.Models.Worklog worklog)
        {
            await jiraApiClient.PostWorklogAsync(issueKey, worklog);
        }

        public async Task StopTrackingIssueAsync(TrackedIssue trackedIssue)
        {
            var trackedIssues = await GetTrackedIssuesAsync();

            var issueToRemove = trackedIssues.First(i => i.IssueKey == trackedIssue.IssueKey);
            trackedIssues.Remove(issueToRemove);

            await SaveTrackedIssuesAsync(trackedIssues);
        }
    }
}
