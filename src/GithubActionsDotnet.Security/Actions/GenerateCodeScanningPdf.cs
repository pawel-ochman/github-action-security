using System.ComponentModel;
using System.Data.Common;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using GithubActionsDotnet.Common.Actions;
using GithubActionsDotnet.Common.Constants;
using GithubActionsDotnet.Common.Service;
using GithubActionsDotnet.Security.Models;
using iText.Html2pdf;
using iText.Kernel.Events;
using iText.Kernel.Pdf; 
using Scriban;
using Scriban.Runtime;

namespace GithubActionsDotnet.Security.Actions;


public class GenerateCodeScanningPdfBuilder : IActionBuilder
{
    private readonly IHttpClientFactory _factory;

    public GenerateCodeScanningPdfBuilder(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public bool Support(string verb) => verb == "generate-code-scanning-pdf";

    public void DisplayHelp(IActionLogger logger)
    {
        logger.Info("generate-code-scanning-pdf - Generate a PDF report of the code scanning alerts");
        logger.Info("  --repository-url - The repository url");
        logger.Info("  --github-token - The github token");
    }

    public IActionCommand Build(string[] args)
    {
        var t= CommandLine.Parser.Default.ParseArguments<GenerateCodeScanningPdfConfiguration>(args);
        if (t.Tag == CommandLine.ParserResultType.NotParsed)
        {
            return new HelpActionCommand(new List<IActionBuilder> { this });
        }
        
        return new GenerateCodeScanningPdf(t.Value, _factory);
    }
}

public class GenerateCodeScanningPdf : IActionCommand
{
    private readonly GenerateCodeScanningPdfConfiguration _configuration;
    private readonly IHttpClientFactory _clientFactory;

    public GenerateCodeScanningPdf(GenerateCodeScanningPdfConfiguration configuration,
        IHttpClientFactory clientFactory)
    {
        _configuration = configuration;
        _clientFactory = clientFactory;
    }

    public async Task RunAsync(IActionLogger logger)
    {
        //var alerts = await GetRecentAlertsAsync(logger);
        //File.WriteAllText("C:\\temp\\alerts.json", JsonSerializer.Serialize(alerts));

        var alerts = JsonSerializer.Deserialize<CodeScanAlerts[]>(File.ReadAllText("C:\\temp\\alerts.json"))!;

        var template = File.ReadAllText(_configuration.TemplateUrl);

        var t = ScriptObject.IsImportable(alerts[0]);
        var obs = alerts.Select(x => ScriptObject.From(x)).ToArray();
        var content = Template.Parse(template);

        var errorLevels = alerts.GroupBy(x => x.Rule.Severity)
            .OrderBy(x => x.Key).ToDictionary(x=>x.Key, x =>x.ToArray());
        var os = ScriptObject.From(errorLevels);

        var payload = new
        {
            Result = obs,
            Date = DateTime.UtcNow.ToString("D"),
            Repository = _configuration.RepositoryUrl,
            ErrorLevels = os,
        };
        var result = content.Render(payload);

        ConverterProperties properties = new ConverterProperties();

        PdfWriter writer = new PdfWriter($"{_configuration.OutputFolder}/document.pdf");

        PdfDocument pdf = new PdfDocument(writer);

        properties.SetBaseUri("https://google.com");
        
        HtmlConverter.ConvertToPdf(new MemoryStream(Encoding.UTF8.GetBytes(result)), pdf, properties);
    }

    private async Task<CodeScanAlerts[]> GetRecentAlertsAsync(IActionLogger logger)
    {
        try
        {
            using var client = _clientFactory.CreateClient(ActionConstants.GithubApiClientName);


            var request = new HttpRequestMessage(HttpMethod.Get, $"{_configuration.RepositoryUrl}/code-scanning/alerts");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _configuration.GithubToken);
            request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<CodeScanAlerts[]>(content)!;
            return data;
        }
        catch (Exception e)
        {
            logger.Error(e.Message);
            throw;
        }
    }
}


public record DismissedBy(
        [property: JsonPropertyName("login")] string Login,
        [property: JsonPropertyName("id")] int? Id,
        [property: JsonPropertyName("node_id")] string NodeId,
        [property: JsonPropertyName("avatar_url")] string AvatarUrl,
        [property: JsonPropertyName("gravatar_id")] string GravatarId,
        [property: JsonPropertyName("url")] string Url,
        [property: JsonPropertyName("html_url")] string HtmlUrl,
        [property: JsonPropertyName("followers_url")] string FollowersUrl,
        [property: JsonPropertyName("following_url")] string FollowingUrl,
        [property: JsonPropertyName("gists_url")] string GistsUrl,
        [property: JsonPropertyName("starred_url")] string StarredUrl,
        [property: JsonPropertyName("subscriptions_url")] string SubscriptionsUrl,
        [property: JsonPropertyName("organizations_url")] string OrganizationsUrl,
        [property: JsonPropertyName("repos_url")] string ReposUrl,
        [property: JsonPropertyName("events_url")] string EventsUrl,
        [property: JsonPropertyName("received_events_url")] string ReceivedEventsUrl,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("site_admin")] bool? SiteAdmin
);

public record Location(
        [property: JsonPropertyName("path")] string Path,
        [property: JsonPropertyName("start_line")] int? StartLine,
        [property: JsonPropertyName("end_line")] int? EndLine,
        [property: JsonPropertyName("start_column")] int? StartColumn,
        [property: JsonPropertyName("end_column")] int? EndColumn
);

public record Message(
        [property: JsonPropertyName("text")] string Text
);

public record MostRecentInstance(
        [property: JsonPropertyName("ref")] string Ref,
        [property: JsonPropertyName("analysis_key")] string AnalysisKey,
        [property: JsonPropertyName("category")] string Category,
        [property: JsonPropertyName("environment")] string Environment,
        [property: JsonPropertyName("state")] string State,
        [property: JsonPropertyName("commit_sha")] string CommitSha,
        [property: JsonPropertyName("message")] Message Message,
        [property: JsonPropertyName("location")] Location Location,
        [property: JsonPropertyName("classifications")] IReadOnlyList<string> Classifications
);

public record CodeScanAlerts( 
        [property: JsonPropertyName("number")] int? Number, 
        [property: JsonPropertyName("created_at")] DateTime? CreatedAt,
        [property: JsonPropertyName("url")] string Url,
        [property: JsonPropertyName("html_url")] string HtmlUrl,
        [property: JsonPropertyName("state")] string State,
        [property: JsonPropertyName("fixed_at")] string FixedAt,
        [property: JsonPropertyName("dismissed_by")] DismissedBy DismissedBy,
        [property: JsonPropertyName("dismissed_at")] DateTime? DismissedAt,
        [property: JsonPropertyName("dismissed_reason")] string DismissedReason,
        [property: JsonPropertyName("dismissed_comment")] string DismissedComment,
        [property: JsonPropertyName("rule")] Rule Rule,
        [property: JsonPropertyName("tool")] Tool Tool,
        [property: JsonPropertyName("most_recent_instance")] MostRecentInstance MostRecentInstance,
        [property: JsonPropertyName("instances_url")] string InstancesUrl
);

public record Rule( 
        [property: JsonPropertyName("id")] string Id, 
        [property: JsonPropertyName("severity")] string Severity, 
        [property: JsonPropertyName("tags")] IReadOnlyList<string> Tags, 
        [property: JsonPropertyName("description")] string Description, 
        [property: JsonPropertyName("name")] string Name
);

public record Tool( 
        [property: JsonPropertyName("name")] string Name, 
        [property: JsonPropertyName("guid")] object Guid, 
        [property: JsonPropertyName("version")] string Version
);

