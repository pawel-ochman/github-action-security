using GithubActionsDotnet.Common.Actions;

namespace GithubActionsDotnet.Common.Service;

public interface IActionResolver
{
    IActionCommand GetCommand(string[] args);
}

public interface IActionLogger
{
    void Info(string message);
    void Error(string message);
    void Warning(string message);
    void Debug(string message);
}

class ActionResolver : IActionResolver
{
    private readonly IEnumerable<IActionBuilder> _builders;

    public ActionResolver(IEnumerable<IActionBuilder> builders)
    {
        _builders = builders;
    }

    public IActionCommand GetCommand(string[] args)
    {
        var verb = args.FirstOrDefault();
        if(string.IsNullOrEmpty(verb))
            return new HelpActionCommand(_builders);

        var builder = _builders.FirstOrDefault(x => x.Support(args[0]));
        if(builder == null)
            return new HelpActionCommand(_builders);

        return builder.Build(args);
    }
}

public class HelpActionCommand : IActionCommand
{
    private readonly IEnumerable<IActionBuilder> _builders;

    public HelpActionCommand(IEnumerable<IActionBuilder> builders)
    {
        _builders = builders;
    }

    public Task RunAsync(IActionLogger logger)
    {
        foreach (var item in _builders)
        {
            item.DisplayHelp(logger);
        }

        return Task.CompletedTask;
    }
}