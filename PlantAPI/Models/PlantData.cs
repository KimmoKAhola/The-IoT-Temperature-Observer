namespace PlantAPI.Models;

public record PlantData
{
    public int Id { get; set; }
    public double Temperature { get; set; }
    public DateTime Timestamp { get; set; }
}