using System.Text.Json.Serialization;

namespace PlantAPI.Models.User;

public record UpdateUserModel
{
    [JsonPropertyName("is_subscriber")]
    public bool IsSubscriber { get; set; }
}
