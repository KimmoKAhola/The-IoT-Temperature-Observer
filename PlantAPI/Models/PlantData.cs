namespace PlantAPI.Models;

public record PlantData
{
    public int Id { get; set; }
    public double Temperature { get; set; }
    public int DHT_Temperature { get; set; }
    public int DHT_Humidity { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Sensor Sensor { get; set; }
}
