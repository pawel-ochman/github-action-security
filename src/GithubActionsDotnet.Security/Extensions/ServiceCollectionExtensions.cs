using GithubActionsDotnet.Common.Actions;
using GithubActionsDotnet.Security.Actions;
using GithubActionsDotnet.Security.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GithubActionsDotnet.Security.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSecurityActions(this IServiceCollection services)
    {
        services.AddSingleton<IActionBuilder, GenerateCodeScanningPdfBuilder>();
        services.AddSingleton<IPdfGeneration, PdfGeneration>();
        return services;
    }
}