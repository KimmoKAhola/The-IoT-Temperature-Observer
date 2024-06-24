using System.Text.Json.Serialization;

namespace PlantAPI.Models;

public class PlantDataDTO
{
    public double Temperature { get; set; }

    [JsonPropertyName("dht_temperature")]
    public int DHT_Temperature { get; set; }

    [JsonPropertyName("dht_humidity")]
    public int DHT_Humidity { get; set; }

    [JsonPropertyName("sensor_id")]
    public string Sensor_Id { get; set; }
}
