using Newtonsoft.Json;

namespace CSharpAPITemplate.Infrastructure.Results.Base;

public class ErrorResult
{
    [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
    public string? Message { get; set; }
        
    [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
    public string? Description { get; set; }
}