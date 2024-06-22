using System.Text.Json.Serialization;

namespace PlantAPI.Controllers;

public class PlantDataModel
{
    public double Temperature { get; set; }

    [JsonIgnore]
    public DateTime Timestamp { get; set; }
}
