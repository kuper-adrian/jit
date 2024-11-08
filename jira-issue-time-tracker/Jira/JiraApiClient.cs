using System.Text;
using System.Text.Json;

namespace jira_issue_time_tracker.Jira
{
    internal interface IJiraApiClient
    {
        Task PostWorklogAsync(string issueKey, Models.Worklog worklog);
    }

    internal class JiraApiClient(HttpClient httpClient) : IJiraApiClient
    {
        public async Task PostWorklogAsync(string issueKey, Models.Worklog worklog)
        {
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(worklog),
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync($"rest/api/3/issue/{issueKey}/worklog", jsonContent);

            response.EnsureSuccessStatusCode();
        }
    }
}
