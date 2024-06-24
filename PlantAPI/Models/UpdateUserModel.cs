using System.Text.Json.Serialization;

namespace PlantAPI.Models;

public record UpdateUserModel
{
    [JsonPropertyName("is_subscriber")]
    public bool IsSubscriber { get; set; }
}
