using Microsoft.EntityFrameworkCore;
using PlantAPI.Models;

namespace PlantAPI;

public class PlantContext : DbContext
{
    public PlantContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<Test> Test { get; set; } 
}