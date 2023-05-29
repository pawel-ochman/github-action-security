using GithubActionsDotnet.Common.Models;
using GithubActionsDotnet.Common.Models.GitHub;
using OneOf;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GithubActionsDotnet.Common.Service;

public class GithubClient: IGithubClient
{
    private readonly HttpClient _client;
    private readonly IActionLogger _logger;

    private static class Routes
    {
        public const string CodeScanningAlerts = "code-scanning/alerts";
        public const string DependabotAlerts = "dependabot/alerts";
    }

    public GithubClient(HttpClient client,
        IActionLogger logger)
    {
        _client = client;
        _logger = logger;
    }

    public Task<OneOf<RequestError, CodeScanAlerts[]>> GetRecentAlertsAsync(string repository, string accessToken)
        => Execute<CodeScanAlerts[]>(repository, accessToken, Routes.CodeScanningAlerts);

    public Task<OneOf<RequestError, DependencyResult[]>> GetDependenciesAlerts(string repository, string accessToken)
        => Execute<DependencyResult[]>(repository, accessToken, Routes.DependabotAlerts);

    private async Task<OneOf<RequestError, TResult>> Execute<TResult>(string repository, string accessToken, string route)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{repository}/{route}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

            var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TResult>(content)!;
            return data;
        }
        catch (Exception e)
        {
            _logger.Warning(e.Message);
            return new RequestError(e.Message);
        }
    }
}

public interface IGithubClient
{
    Task<OneOf<RequestError, CodeScanAlerts[]>> GetRecentAlertsAsync(string repository, string accessToken);
    Task<OneOf<RequestError, DependencyResult[]>> GetDependenciesAlerts(string repository, string accessToken);
}