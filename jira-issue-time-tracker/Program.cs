using jira_issue_time_tracker.Commands;
using jira_issue_time_tracker.DependencyInjection;
using jira_issue_time_tracker.Extensions;
using jira_issue_time_tracker.Jira;
using jira_issue_time_tracker.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace jira_issue_time_tracker
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddHttpClient<IJiraApiClient, JiraApiClient>(client =>
            {
                DotNetEnv.Env.TraversePath().Load();
                var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();

                client.BaseAddress = new Uri(uriString: configuration["BASE_URL"] ?? string.Empty);

                var userMail = configuration["USER_MAIL"];
                var apiToken = configuration["API_TOKEN"];

                var authorizationHeaderValue =
                    "Basic " + $"{userMail}:{apiToken}".ToBase64EncodedString();

                client.DefaultRequestHeaders.Add("Authorization", authorizationHeaderValue);
            });
            services.AddScoped<IJiraIssueService, JiraIssueService>();

            // Create a type registrar and register any dependencies.
            // A type registrar is an adapter for a DI framework.
            var registrar = new TypeRegistrar(services);

            var app = new CommandApp(registrar);
            app.Configure(config =>
            {
                config.AddCommand<StartCommand>("start");
                config.AddCommand<StopCommand>("stop");
                config.AddCommand<StatusCommand>("status");
            });

            await app.RunAsync(args);
        }
    }
}
