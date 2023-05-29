using GithubActionsDotnet.Common.Actions;
using GithubActionsDotnet.Common.Models.GitHub;
using GithubActionsDotnet.Common.Service;
using GithubActionsDotnet.Security.Models;
using GithubActionsDotnet.Security.Services;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using OneOf.Types;
using Scriban;
using Scriban.Runtime;
using System.Text;

namespace GithubActionsDotnet.Security.Actions;

public class GenerateCodeScanningPdfBuilder : IActionBuilder
{
    private readonly IGithubClient _client;
    private readonly IPdfGeneration _pdfGeneration;

    public GenerateCodeScanningPdfBuilder(IGithubClient client,
        IPdfGeneration pdfGeneration)
    {
        _client = client;
        _pdfGeneration = pdfGeneration;
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
        
        return new GenerateCodeScanningPdf(t.Value, 
            _client, 
            _pdfGeneration);
    }
}

public class GenerateCodeScanningPdf : IActionCommand
{
    private readonly GenerateCodeScanningPdfConfiguration _configuration;
    private readonly IGithubClient _client;
    private readonly IPdfGeneration _pdfGeneration;

    public GenerateCodeScanningPdf(GenerateCodeScanningPdfConfiguration configuration,
        IGithubClient client,
        IPdfGeneration pdfGeneration)
    {
        _configuration = configuration;
        _client = client;
        _pdfGeneration = pdfGeneration;
    }

    public async Task RunAsync(IActionLogger logger)
    {  
        var payload = new TemplatePayload
        {
            Date = DateTime.UtcNow.ToString("D"),
            Repository = _configuration.RepositoryUrl,
        };

        await Task.WhenAll(
            PullCodeScanInformation(payload, logger),
            PullDependencyInformation(payload, logger));

        var template = await File.ReadAllTextAsync(_configuration.TemplateUrl);
        var content = Template.Parse(template);
        var result = await content.RenderAsync(payload);

        _pdfGeneration.GeneratePdfFromHtml(result, $"{_configuration.OutputFolder}/{_configuration.FileName}");
    }

    private async Task PullDependencyInformation(TemplatePayload payload, IActionLogger logger)
    { 
        var dependencies = await _client.GetDependenciesAlerts(
            _configuration.RepositoryUrl,
            _configuration.GithubToken);

        dependencies.Switch(
            err =>
            {
                payload.DependenciesStats = ScriptObject.From(new AlertsSummary<int>());
                payload.Dependencies = ScriptObject.From(new AlertsSummary<DependencyResult[]>
                {
                    Critical = Array.Empty<DependencyResult>(),
                    Error = Array.Empty<DependencyResult>(),
                    High = Array.Empty<DependencyResult>(),
                    Low = Array.Empty<DependencyResult>(),
                    Medium = Array.Empty<DependencyResult>(),
                    Warning = Array.Empty<DependencyResult>(),
                    Note = Array.Empty<DependencyResult>(),
                });
                payload.DependenciesAlerts = Array.Empty<ScriptObject>();
                logger.Warning($"Can't pull dependencies data: {err.Message}");
            },
            result =>
            {
                payload.DependenciesAlerts = result.Select(ScriptObject.From).ToArray();

                var errorLevels = result
                    .GroupBy(x => x.SecurityAdvisory.Severity)
                    .OrderBy(x => x.Key)
                    .ToDictionary(x => x.Key, x => x.ToArray());

                payload.DependenciesStats = ScriptObject.From(new AlertsSummary<int>
                {
                    Critical = errorLevels.TryGetValue(Severity.Critical, array => array.Length, () => 0),
                    Error = errorLevels.TryGetValue(Severity.Error, array => array.Length, () => 0),
                    High = errorLevels.TryGetValue(Severity.High, array => array.Length, () => 0),
                    Low = errorLevels.TryGetValue(Severity.Low, array => array.Length, () => 0),
                    Medium = errorLevels.TryGetValue(Severity.Medium, array => array.Length, () => 0),
                    Warning = errorLevels.TryGetValue(Severity.Warning, array => array.Length, () => 0),
                    Note = errorLevels.TryGetValue(Severity.Note, array => array.Length, () => 0),
                });
                payload.Dependencies = ScriptObject.From(new AlertsSummary<DependencyResult[]>
                {
                    Critical = errorLevels.TryGetValue(Severity.Critical, array => array, Array.Empty<DependencyResult>),
                    Error = errorLevels.TryGetValue(Severity.Error, array => array, Array.Empty<DependencyResult>),
                    High = errorLevels.TryGetValue(Severity.High, array => array, Array.Empty<DependencyResult>),
                    Low = errorLevels.TryGetValue(Severity.Low, array => array, Array.Empty<DependencyResult>),
                    Medium = errorLevels.TryGetValue(Severity.Medium, array => array, Array.Empty<DependencyResult>),
                    Warning = errorLevels.TryGetValue(Severity.Warning, array => array, Array.Empty<DependencyResult>),
                    Note = errorLevels.TryGetValue(Severity.Note, array => array, Array.Empty<DependencyResult>),
                });
            });
    }

    private async Task PullCodeScanInformation(TemplatePayload payload, IActionLogger logger)
    {
        var codeScanningAlerts = await _client.GetRecentAlertsAsync(
            _configuration.RepositoryUrl,
            _configuration.GithubToken);

        codeScanningAlerts.Switch(
            err =>
            {
                payload.IssuesStats = ScriptObject.From(new AlertsSummary<int>());
                payload.Issues = ScriptObject.From(new AlertsSummary<CodeScanAlerts[]>
                {
                    Critical = Array.Empty<CodeScanAlerts>(),
                    Error = Array.Empty<CodeScanAlerts>(),
                    High = Array.Empty<CodeScanAlerts>(),
                    Low = Array.Empty<CodeScanAlerts>(),
                    Medium = Array.Empty<CodeScanAlerts>(),
                    Warning = Array.Empty<CodeScanAlerts>(),
                    Note = Array.Empty<CodeScanAlerts>(),
                });
                payload.CodeScanAlerts = Array.Empty<ScriptObject>();
                logger.Warning($"Can't pull code scan data: {err.Message}");
            },
            result =>
            {
                payload.CodeScanAlerts = result.Select(ScriptObject.From).ToArray();

                var errorLevels = result
                    .GroupBy(x => x.Rule.Severity)
                    .OrderBy(x => x.Key)
                    .ToDictionary(x => x.Key, x => x.ToArray());

                payload.IssuesStats = ScriptObject.From(new AlertsSummary<int>
                {
                    Critical = errorLevels.TryGetValue(Severity.Critical, array => array.Length, () => 0),
                    Error = errorLevels.TryGetValue(Severity.Error, array => array.Length, () => 0),
                    High = errorLevels.TryGetValue(Severity.High, array => array.Length, () => 0),
                    Low = errorLevels.TryGetValue(Severity.Low, array => array.Length, () => 0),
                    Medium = errorLevels.TryGetValue(Severity.Medium, array => array.Length, () => 0),
                    Warning = errorLevels.TryGetValue(Severity.Warning, array => array.Length, () => 0),
                    Note = errorLevels.TryGetValue(Severity.Note, array => array.Length, () => 0),
                });
                payload.Issues = ScriptObject.From(new AlertsSummary<CodeScanAlerts[]>
                {
                    Critical = errorLevels.TryGetValue(Severity.Critical, array => array, Array.Empty<CodeScanAlerts>),
                    Error = errorLevels.TryGetValue(Severity.Error, array => array, Array.Empty<CodeScanAlerts>),
                    High = errorLevels.TryGetValue(Severity.High, array => array, Array.Empty<CodeScanAlerts>),
                    Low = errorLevels.TryGetValue(Severity.Low, array => array, Array.Empty<CodeScanAlerts>),
                    Medium = errorLevels.TryGetValue(Severity.Medium, array => array, Array.Empty<CodeScanAlerts>),
                    Warning = errorLevels.TryGetValue(Severity.Warning, array => array, Array.Empty<CodeScanAlerts>),
                    Note = errorLevels.TryGetValue(Severity.Note, array => array, Array.Empty<CodeScanAlerts>),
                });
            });
    }
     
    public class AlertsSummary<TItem>
    {
        public TItem? Critical { get; set; }
        public TItem? Error { get; set; }
        public TItem? High { get; set; }
        public TItem? Low { get; set; }
        public TItem? Medium { get; set; }
        public TItem? Warning { get; set; }
        public TItem? Note { get; set; }
    }

    public class TemplatePayload 
    {
        public ScriptObject[]? CodeScanAlerts { get; set; }
        public ScriptObject[]? DependenciesAlerts { get; set; }
        public string? Date { get; set; }
        public string? Repository { get; set; }
        public ScriptObject? IssuesStats { get; set; }
        public ScriptObject? Issues { get; set; }
        public ScriptObject? DependenciesStats { get; set; }
        public ScriptObject? Dependencies { get; set; }
    }
}

public static class Extensions
{
    public static TResult TryGetValue<TKey,TValue, TResult>(this Dictionary<TKey, TValue> al, 
        TKey key,
        Func<TValue, TResult> whenExist,
        Func<TResult> whenNotExist) where TKey : notnull
    {
        return al.TryGetValue(key, out var t) ? whenExist(t) : whenNotExist();
    }
}




