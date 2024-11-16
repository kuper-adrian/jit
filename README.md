# JIT - Track time of Jira issues
JIT is a small CLI tool, that helps you to track the time spent working on your Jira issues.

## Features
- Track time spent working on Jira tickets using a developer-friendly command line interface
- Track time for multiple issues at once

## Install
For now, you are required to clone the repository and build the tool from source. .NET 8 is required. 
```bash
git clone https://github.com/kuper-adrian/jit.git
cd jit
dotnet build # check bin folder for executable
```
In order to authenticate against the Jira API you have to create an API Token as explained 
[here](https://support.atlassian.com/atlassian-account/docs/manage-api-tokens-for-your-atlassian-account/). Then, 
inside the folder of the JIT executable, create a `.env` file with the following content:
```dotenv
BASE_URL=<PASTE BASE URL OF YOUR JIRA INSTANCE>
USER_MAIL=<PASTE MAIL ADRESS OF YOUR JIRA ACCOUNT>
API_TOKEN=<PASTE API TOKEN CREATED EARLIER>
```


## Usage
```
USAGE:
    jit.dll [OPTIONS] <COMMAND>

OPTIONS:
    -h, --help    Prints help information

COMMANDS:
    start <ISSUE_KEY>    Start tracking time for an issue
    stop                 Stop tracking time for an issue
    status               Display status information about tracked issues
```

## Changelog
TODO

## License
MIT
