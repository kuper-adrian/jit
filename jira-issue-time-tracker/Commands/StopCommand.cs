using System.ComponentModel;
using Humanizer;
using jira_issue_time_tracker.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using static jira_issue_time_tracker.Services.JiraIssueService;

namespace jira_issue_time_tracker.Commands
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class StopCommand(IJiraIssueService jiraIssueService)
        : AsyncCommand<StopCommand.Settings>
    {
        // ReSharper disable once ClassNeverInstantiated.Global
        public sealed class Settings : CommandSettings
        {
            [Description("Key of issue to track time for.")]
            [CommandArgument(0, "[ISSUE_KEY]")]
            public string? IssueKey { get; init; }

            [Description("Skip confirmation step.")]
            [CommandOption("-Y")]
            public bool IsPreConfirmed { get; set; } = false;

            [Description("Cancel tracking without adding time to issue.")]
            [CommandOption("-c|--cancel")]
            public bool IsCancelled { get; set; } = false;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var returnValue = 0;

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
            else if (settings.IssueKey != null)
            {
                var trackedIssue = trackedIssues.FirstOrDefault(i =>
                    i.IssueKey == settings.IssueKey
                );

                if (trackedIssue == null)
                {
                    AnsiConsole.Write(
                        new Panel(new Text($"Not tracking issue with key {settings.IssueKey}!"))
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
                    await UpdateIssueAsync(trackedIssue, settings);
                }
            }
            else if (trackedIssues.Count == 1)
            {
                await UpdateIssueAsync(trackedIssues.First(), settings);
            }
            else
            {
                var selectionPrompt = new SelectionPrompt<TrackedIssue>()
                    .Title("Select [green]issue[/]")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more issues)[/]")
                    .AddChoices(trackedIssues);
                selectionPrompt.Converter = issue =>
                    $"[green][[{issue.IssueKey}]][/] {issue.Summary}";
                var selectedIssue = AnsiConsole.Prompt(selectionPrompt);

                await UpdateIssueAsync(selectedIssue, settings);
            }

            return returnValue;
        }

        private async Task UpdateIssueAsync(TrackedIssue issue, Settings settings)
        {
            // minimum time to log is one minute
            var elapsedTimeSeconds = issue.ElapsedTime.Seconds;
            if (elapsedTimeSeconds < 60)
            {
                elapsedTimeSeconds = 60;
            }

            TextPrompt<bool> confirmationPrompt;

            if (settings.IsCancelled)
            {
                confirmationPrompt = new TextPrompt<bool>(
                    $"Are you sure about cancelling tracking issue [green][[{issue.IssueKey}]][/] \"{issue.Summary}\" (Elapsed time: [blue]{issue.ElapsedTime.Humanize(2)}[/])?"
                )
                    .AddChoice(true)
                    .AddChoice(false)
                    .DefaultValue(false)
                    .WithConverter(choice => choice ? "y" : "N");
            }
            else
            {
                confirmationPrompt = new TextPrompt<bool>(
                    $"Adding [blue]{issue.ElapsedTime.Humanize(2)}[/] to issue [green][[{issue.IssueKey}]][/] \"{issue.Summary}\". Confirm?"
                )
                    .AddChoice(true)
                    .AddChoice(false)
                    .DefaultValue(true)
                    .WithConverter(choice => choice ? "Y" : "n");
            }

            // Ask the user to confirm
            var isConfirmed = !settings.IsPreConfirmed
                ? AnsiConsole.Prompt(confirmationPrompt)
                : settings.IsPreConfirmed;

            if (isConfirmed)
            {
                await AnsiConsole
                    .Status()
                    .StartAsync(
                        $"Trying to update issue {issue.IssueKey}...",
                        async ctx =>
                        {
                            try
                            {
                                if (!settings.IsCancelled)
                                {
                                    await jiraIssueService.UpdateWorklogAsync(
                                        issue.IssueKey,
                                        new Jira.Models.Worklog()
                                        {
                                            TimeSpentSeconds = elapsedTimeSeconds,
                                        }
                                    );
                                }

                                await jiraIssueService.StopTrackingIssueAsync(issue);
                            }
                            catch (Exception ex)
                            {
                                // TODO write log
                                AnsiConsole.Write(
                                    new Panel(ex.GetRenderable()) { Expand = true }
                                        .Header("Error")
                                        .RoundedBorder()
                                        .BorderColor(Color.Red)
                                );
                            }
                        }
                    );

                if (!settings.IsCancelled)
                {
                    AnsiConsole.Write(
                        new Panel(new Text("Successfully added time to issue!")) { Expand = true }
                            .Header("Success")
                            .RoundedBorder()
                            .BorderColor(Color.Green)
                    );
                }
                else
                {
                    AnsiConsole.Write(
                        new Panel(new Text("Cancelled!")) { Expand = true }
                            .Header("Success")
                            .RoundedBorder()
                            .BorderColor(Color.Green)
                    );
                }
            }
        }
    }
}
