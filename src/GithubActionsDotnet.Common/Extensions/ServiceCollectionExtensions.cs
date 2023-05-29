using GithubActionsDotnet.Common.Constants;
using GithubActionsDotnet.Common.Models;
using GithubActionsDotnet.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace GithubActionsDotnet.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services, BaseActionConfig config)
    {
        services.AddSingleton<IActionLogger, ActionLogger>();
        services.AddSingleton<IActionResolver, ActionResolver>();
        services.AddHttpClient<IGithubClient, GithubClient>(client =>
        {
            client.BaseAddress = new Uri(config.BaseAddress);
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
        });
        return services;
    }
}