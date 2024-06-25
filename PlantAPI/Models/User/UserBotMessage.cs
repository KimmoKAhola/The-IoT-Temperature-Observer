using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantAPI.Models.User;

public record UserBotMessage
{
    [Key]
    public int MessageId { get; set; }
    public string UserChatId { get; set; }

    [ForeignKey(nameof(UserChatId))]
    public virtual User User { get; set; }

    public string Content { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
