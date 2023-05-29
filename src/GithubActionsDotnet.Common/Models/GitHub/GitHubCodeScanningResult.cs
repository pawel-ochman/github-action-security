using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GithubActionsDotnet.Common.Models.GitHub;

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

public enum Severity
{
    Critical,
    High,
    Medium,
    Low,
    Warning,
    Note,
    Error
}

public record Rule(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("severity")] Severity Severity,
        [property: JsonPropertyName("tags")] IReadOnlyList<string> Tags,
        [property: JsonPropertyName("description")] string Description,
        [property: JsonPropertyName("name")] string Name
);

public record Tool(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("guid")] object Guid,
        [property: JsonPropertyName("version")] string Version
);

// Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
public class Cvss
{
    [JsonProperty("vector_string")]
    public string VectorString { get; set; }

    [JsonProperty("score")]
    public double Score { get; set; }
}

public class Cwe
{
    [JsonProperty("cwe_id")]
    public string CweId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Dependency
{
    [JsonProperty("package")]
    public Package Package { get; set; }

    [JsonProperty("manifest_path")]
    public string ManifestPath { get; set; }

    [JsonProperty("scope")]
    public string Scope { get; set; }
} 

public class FirstPatchedVersion
{
    [JsonProperty("identifier")]
    public string Identifier { get; set; }
}

public class Identifier
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("value")]
    public string Value { get; set; }
}

public class Package
{
    [JsonProperty("ecosystem")]
    public string Ecosystem { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class Reference
{
    [JsonProperty("url")]
    public string Url { get; set; }
}

public class DependencyResult
{
    [JsonProperty("number")]
    public int Number { get; set; }

    [JsonProperty("state")]
    public string State { get; set; }

    [JsonProperty("dependency")]
    public Dependency Dependency { get; set; }

    [JsonProperty("security_advisory")]
    public SecurityAdvisory SecurityAdvisory { get; set; }

    [JsonProperty("security_vulnerability")]
    public SecurityVulnerability SecurityVulnerability { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("html_url")]
    public string HtmlUrl { get; set; }

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonProperty("dismissed_at")]
    public DateTime? DismissedAt { get; set; }

    [JsonProperty("dismissed_by")]
    public DismissedBy DismissedBy { get; set; }

    [JsonProperty("dismissed_reason")]
    public string DismissedReason { get; set; }

    [JsonProperty("dismissed_comment")]
    public string DismissedComment { get; set; }

    [JsonProperty("fixed_at")]
    public object FixedAt { get; set; }
}

public class SecurityAdvisory
{
    [JsonProperty("ghsa_id")]
    public string GhsaId { get; set; }

    [JsonProperty("cve_id")]
    public string CveId { get; set; }

    [JsonProperty("summary")]
    public string Summary { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("vulnerabilities")]
    public List<Vulnerability> Vulnerabilities { get; set; }

    [JsonProperty("severity")]
    public Severity Severity { get; set; }

    [JsonProperty("cvss")]
    public Cvss Cvss { get; set; }

    [JsonProperty("cwes")]
    public List<Cwe> Cwes { get; set; }

    [JsonProperty("identifiers")]
    public List<Identifier> Identifiers { get; set; }

    [JsonProperty("references")]
    public List<Reference> References { get; set; }

    [JsonProperty("published_at")]
    public DateTime PublishedAt { get; set; }

    [JsonProperty("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonProperty("withdrawn_at")]
    public object WithdrawnAt { get; set; }
}

public class SecurityVulnerability
{
    [JsonProperty("package")]
    public Package Package { get; set; }

    [JsonProperty("severity")]
    public string Severity { get; set; }

    [JsonProperty("vulnerable_version_range")]
    public string VulnerableVersionRange { get; set; }

    [JsonProperty("first_patched_version")]
    public FirstPatchedVersion FirstPatchedVersion { get; set; }
}

public class Vulnerability
{
    [JsonProperty("package")]
    public Package Package { get; set; }

    [JsonProperty("severity")]
    public string Severity { get; set; }

    [JsonProperty("vulnerable_version_range")]
    public string VulnerableVersionRange { get; set; }

    [JsonProperty("first_patched_version")]
    public FirstPatchedVersion FirstPatchedVersion { get; set; }
}

