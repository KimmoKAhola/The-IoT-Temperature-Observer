using System.Text.Json.Serialization;

namespace PlantAPI.Models.Message;

public record MessageDTO
{
    [JsonPropertyName("userChatId")]
    public string UserChatId { get; set; }

    public string Content { get; set; }
    public string FirstName { get; set; }
}
