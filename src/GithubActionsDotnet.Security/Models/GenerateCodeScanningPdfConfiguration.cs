using CommandLine;
using GithubActionsDotnet.Common.Models;

namespace GithubActionsDotnet.Security.Models;

[Verb("generate-code-scanning-pdf", 
    HelpText = "Generate pdf from code scanning")]
public class GenerateCodeScanningPdfConfiguration: BaseActionConfig
{
    [Option(Required = true,
            HelpText = "Github token")]
    public string GithubToken { get; set; }

    [Option(Required = true,
            HelpText = "Output folder")]
    public string OutputFolder { get; set; }

    [Option(Required = false,
        HelpText = "Template url")]
    public string TemplateUrl { get; set; }

    [Option(Required = false,
        HelpText = "Template content")]
    public string TemplateContent { get; set; }

    [Option(Required = false,
        HelpText = "Repository url")]
    public string RepositoryUrl { get; set; }

    [Option(Required = false,
        HelpText = "Branch name i.e. refs/heads/master")]
    public string Branch { get; set; }
}