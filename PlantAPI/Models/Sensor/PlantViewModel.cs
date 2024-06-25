namespace PlantAPI.Models.Sensor;

public class PlantViewModel
{
    public double Temperature { get; set; }
    public int DHT_Temperature { get; set; }
    public int DHT_Humidity { get; set; }
    public DateTime Timestamp { get; set; }
}
