using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PlantAPI.Models.User;

public record User
{
    [Key]
    public string UserChatId { get; set; }
    public string FirstName { get; set; }

    [JsonIgnore]
    public bool IsSubscriber { get; set; }
    public virtual ICollection<UserBotMessage> Messages { get; set; } = new List<UserBotMessage>();
}
