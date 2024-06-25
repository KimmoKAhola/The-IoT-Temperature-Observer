using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PlantAPI.Models.Sensor;

public class PlantDataDTO
{
    [Range(-50, 100)]
    public double Temperature { get; set; }

    [JsonPropertyName("dht_temperature")]
    [Range(-50, 100)]
    public int DHT_Temperature { get; set; }

    [JsonPropertyName("dht_humidity")]
    [Range(-50, 100)]
    public int DHT_Humidity { get; set; }

    [JsonPropertyName("sensor_id")]
    public string Sensor_Id { get; set; }
}
