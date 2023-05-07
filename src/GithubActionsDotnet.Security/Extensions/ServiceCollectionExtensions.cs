using GithubActionsDotnet.Common.Actions;
using GithubActionsDotnet.Security.Actions;
using Microsoft.Extensions.DependencyInjection;

namespace GithubActionsDotnet.Security.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSecurityActions(this IServiceCollection services)
    {
        services.AddSingleton<IActionBuilder, GenerateCodeScanningPdfBuilder>();
        return services;
    }
}