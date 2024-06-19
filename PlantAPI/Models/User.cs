using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PlantAPI.Models;

public record User
{
    [Key]
    [JsonIgnore]
    public int Id { get; set; }
    public string FirstName { get; set; }
    
    [JsonIgnore]
    public int NumberOfMessagesSent { get; set; }
    public string UserChatId { get; set; }
}