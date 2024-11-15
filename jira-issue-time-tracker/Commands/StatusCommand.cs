using Humanizer;
using jira_issue_time_tracker.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace jira_issue_time_tracker.Commands
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class StatusCommand(IJiraIssueService jiraIssueService)
        : AsyncCommand<StatusCommand.Settings>
    {
        // ReSharper disable once ClassNeverInstantiated.Global
        public sealed class Settings : CommandSettings { }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var trackedIssues = await jiraIssueService.GetTrackedIssuesAsync();

            if (trackedIssues.Count == 0)
            {
                AnsiConsole.Write(
                    new Panel(new Text("Not tracking any issue!")) { Expand = true }
                        .Header("Warning")
                        .RoundedBorder()
                        .BorderColor(Color.Yellow)
                );
            }
            else
            {
                AnsiConsole.Write(
                    new Panel(
                        new Rows(
                            trackedIssues.Select(i => new Markup(
                                $"[green][[{i.IssueKey}]][/] \"{i.Summary}\", Elapsed time: [blue]{i.ElapsedTime.Humanize(2)}[/] (since: {i.TrackedSince.ToString()})"
                            ))
                        )
                    )
                    {
                        Expand = true,
                    }
                        .Header("Status")
                        .RoundedBorder()
                        .BorderColor(Color.Green)
                );
            }

            return 0;
        }
    }
}
