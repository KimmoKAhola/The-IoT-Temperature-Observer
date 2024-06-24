﻿using System.ComponentModel.DataAnnotations;

namespace PlantAPI.Models;

public class Sensor
{
    [Key]
    public string Id { get; set; }

    public string Location { get; set; }
}
