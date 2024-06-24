using Microsoft.EntityFrameworkCore;
using PlantAPI.Models;

namespace PlantAPI;

public class PlantContext : DbContext
{
    public PlantContext(DbContextOptions options)
        : base(options) { }

    public DbSet<PlantData> PlantData { get; set; }
    public DbSet<Message> UserMessages { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Sensor> Sensors { get; set; }
}
