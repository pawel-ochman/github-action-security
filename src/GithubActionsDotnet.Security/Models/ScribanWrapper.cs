using Scriban.Runtime;

namespace GithubActionsDotnet.Security.Models;

public class ScribanWrapper<TModel>: ScriptObject
{
    public ScribanWrapper(TModel model)
    { 
        this.Import(model);
    } 
}