using System.ComponentModel;
using jira_issue_time_tracker.Jira.Models;
using jira_issue_time_tracker.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace jira_issue_time_tracker.Commands
{
    [Description("Start tracking time for an issue.")]
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class StartCommand(IJiraIssueService jiraIssueService)
        : AsyncCommand<StartCommand.Settings>
    {
        // ReSharper disable once ClassNeverInstantiated.Global
        public sealed class Settings : CommandSettings
        {
            [Description("Key of issue to track time for.")]
            [CommandArgument(0, "<ISSUE_KEY>")]
            public string IssueKey { get; init; } = string.Empty;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var returnValue = 0;
            Issue? issue = null;

            await AnsiConsole
                .Status()
                .StartAsync(
                    "Fetching issue...",
                    async ctx =>
                    {
                        issue = await jiraIssueService.GetIssueAsync(settings.IssueKey);
                    }
                );

            if (issue != null)
            {
                var trackedIssues = await jiraIssueService.GetTrackedIssuesAsync();
                if (trackedIssues.Any(i => i.IssueKey == issue.Key))
                {
                    AnsiConsole.Write(
                        new Panel(
                            new Markup(
                                $"Already tracking [green][[{settings.IssueKey}]][/]: {issue.Fields.Summary}"
                            )
                        )
                        {
                            Expand = true,
                        }
                            .Header("Warning")
                            .RoundedBorder()
                            .BorderColor(Color.Yellow)
                    );
                }
                else
                {
                    await AnsiConsole
                        .Status()
                        .StartAsync(
                            "Start tracking issue...",
                            async ctx =>
                            {
                                try
                                {
                                    await jiraIssueService.StartTrackingIssueAsync(
                                        issue.Key,
                                        issue.Fields.Summary
                                    );

                                    AnsiConsole.Write(
                                        new Panel(
                                            new Markup(
                                                $"Started tracking [green][[{settings.IssueKey}]][/]: {issue.Fields.Summary}"
                                            )
                                        )
                                        {
                                            Expand = true,
                                        }
                                            .Header("Success")
                                            .RoundedBorder()
                                            .BorderColor(Color.Green)
                                    );
                                }
                                catch (Exception ex)
                                {
                                    // TODO write log
                                    AnsiConsole.Write(
                                        new Panel(ex.GetRenderable()) { Expand = true }
                                            .Header("ERROR")
                                            .RoundedBorder()
                                            .BorderColor(Color.Red)
                                    );
                                    returnValue = 1;
                                }
                            }
                        );
                }
            }

            return returnValue;
        }
    }
}
