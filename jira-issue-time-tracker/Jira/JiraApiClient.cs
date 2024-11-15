using System.Text;
using System.Text.Json;
using jira_issue_time_tracker.Jira.Models;

namespace jira_issue_time_tracker.Jira
{
    internal interface IJiraApiClient
    {
        Task<Issue?> GetIssueAsync(string issueKey);
        Task PostWorklogAsync(string issueKey, Worklog worklog);
    }

    internal class JiraApiClient(HttpClient httpClient) : IJiraApiClient
    {
        public async Task<Issue?> GetIssueAsync(string issueKey)
        {
            var response = await httpClient.GetAsync($"rest/api/3/issue/{issueKey}");

            response.EnsureSuccessStatusCode();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var issue = await JsonSerializer.DeserializeAsync<Issue>(
                await response.Content.ReadAsStreamAsync(),
                options
            );
            return issue;
        }

        public async Task PostWorklogAsync(string issueKey, Models.Worklog worklog)
        {
            using StringContent jsonContent =
                new(JsonSerializer.Serialize(worklog), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(
                $"rest/api/3/issue/{issueKey}/worklog",
                jsonContent
            );

            response.EnsureSuccessStatusCode();
        }
    }
}
