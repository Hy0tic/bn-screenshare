using System.Text.Json.Serialization;

namespace BnScreenshareAPI;

public class BuildInfo
{
    [JsonPropertyName("buildName")]
    public required string BuildName { get; set; }
    [JsonPropertyName("hash")]
    public required string Hash { get; set; }
    [JsonPropertyName("branch")]
    public required string Branch { get; set; }
    [JsonPropertyName("buildDate")]
    public required string BuildDate { get; set; }
}
