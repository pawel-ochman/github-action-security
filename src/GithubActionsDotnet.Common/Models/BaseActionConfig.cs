using CommandLine;

namespace GithubActionsDotnet.Common.Models;

public class BaseActionConfig
{
    [Option(
        Default = false,
        HelpText = "Prints all messages to standard output.")]
    public bool Verbose { get; set; }

    [Option(
        Default = "https://api.github.com",
        HelpText = "Set default github api address")]
    public string BaseAddress { get; set; }
}