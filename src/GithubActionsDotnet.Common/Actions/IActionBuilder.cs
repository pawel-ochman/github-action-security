using GithubActionsDotnet.Common.Service;

namespace GithubActionsDotnet.Common.Actions;

public interface IActionBuilder
{
    bool Support(string verb);
    void DisplayHelp(IActionLogger logger);
    IActionCommand Build(string[] args);
}