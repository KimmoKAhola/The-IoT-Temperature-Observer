using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PlantAPI.Models;

public record User
{
    [Key]
    public string UserChatId { get; set; }
    public string FirstName { get; set; }

    [JsonIgnore]
    public bool IsSubscriber { get; set; }
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
