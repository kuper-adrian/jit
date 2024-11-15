namespace jira_issue_time_tracker.Jira.Models;

public class Issue
{
    public string Id { get; set; }
    public string Key { get; set; }

    public IssueFields Fields { get; set; }
}

public class IssueFields
{
    public string Summary { get; set; }
}
