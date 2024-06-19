namespace PlantAPI.Models;

public record Test
{
    public int Id { get; set; }
    public double Temperature { get; set; }
    public DateTime Timestamp { get; set; }
}