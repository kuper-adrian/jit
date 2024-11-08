using Spectre.Console.Cli;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jira_issue_time_tracker.Commands
{
    internal class StopCommand : Command<StopCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            AnsiConsole.MarkupInterpolated($"[blue]Stopped tracking time for issue [/]");

            return 0;
        }
    }
}
