using jira_issue_time_tracker.Jira;

namespace jira_issue_time_tracker.Services
{
    internal interface IJiraIssueService
    {
        Task UpdateWorklogAsync(string issueKey, Jira.Models.Worklog worklog);
    }

    internal class JiraIssueService(IJiraApiClient jiraApiClient) : IJiraIssueService
    {
        public async Task UpdateWorklogAsync(string issueKey, Jira.Models.Worklog worklog)
        {
            await jiraApiClient.PostWorklogAsync(issueKey, worklog);
        }
    }
}
