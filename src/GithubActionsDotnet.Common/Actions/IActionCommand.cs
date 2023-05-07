using GithubActionsDotnet.Common.Service;

namespace GithubActionsDotnet.Common.Actions;

public interface IActionCommand
{
    Task RunAsync(IActionLogger logger);
}