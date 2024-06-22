using System.Text.Json.Serialization;

namespace PlantAPI.Models;

public record MessageDTO
{
    [JsonPropertyName("userChatId")]
    public string UserChatId { get; set; }

    public string Content { get; set; }
    public string FirstName { get; set; }
}
