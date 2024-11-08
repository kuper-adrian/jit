using System.ComponentModel;
using jira_issue_time_tracker.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace jira_issue_time_tracker.Commands
{
    internal class StartCommand : AsyncCommand<StartCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            [Description("Key of issue to track time for.")]
            [CommandArgument(0, "<ISSUE_KEY>")]
            public string? IssueKey { get; init; }
        }

        private readonly IJiraIssueService _jiraIssueService;

        public StartCommand(IJiraIssueService jiraIssueService)
        {
            _jiraIssueService = jiraIssueService;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            await GetIssueExample(settings.IssueKey!);

            return 0;
        }

        private async Task GetIssueExample(string issueKey)
        {
            AnsiConsole.MarkupInterpolated(
                $"[blue]Start tracking time for issue [/] -> [green]{issueKey}[/]\n"
            );

            await AnsiConsole
                .Status()
                .StartAsync(
                    "Trying to update issue...",
                    async ctx =>
                    {
                        try
                        {
                            await _jiraIssueService.UpdateWorklogAsync(
                                issueKey,
                                new Jira.Models.Worklog() { TimeSpentSeconds = 12000 }
                            );

                            AnsiConsole.Write(
                                new Panel(new Text("Updated!")) { Expand = true }
                                    .Header("Did it work?")
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
                        }
                    }
                );
        }
    }
}
