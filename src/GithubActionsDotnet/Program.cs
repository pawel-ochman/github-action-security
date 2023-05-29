using System.Text;
using CommandLine; 
using GithubActionsDotnet.Common.Extensions;
using GithubActionsDotnet.Common.Models;
using GithubActionsDotnet.Common.Service;
using GithubActionsDotnet.Security.Extensions;
using GithubActionsDotnet.Security.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static CommandLine.Parser;


var debug = new string[]
{
    "generate-code-scanning-pdf",
    "--repositoryurl",
    "/repos/pawel-ochman/test-sast",
    "--githubtoken",
    "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",
    "--outputfolder",
    "c:\\temp",
    "--templateurl",
    "C:\\Projects\\github-action-security\\src\\GithubActionsDotnet\\Templates\\default.html"
};

var initArguments = Default.ParseArguments<BaseActionConfig>(args);

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) => services
        .AddLogging()
        .AddCommonServices(initArguments.Value)
        .AddSecurityActions())
    .Build();

static TService Get<TService>(IHost host)
    where TService : notnull =>
    host.Services.GetRequiredService<TService>();

var logger = Get<IActionLogger>(host);

await Get<IActionResolver>(host)
    .GetCommand(debug)
    .RunAsync(logger);