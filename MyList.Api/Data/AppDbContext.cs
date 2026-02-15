using Microsoft.EntityFrameworkCore;
using MyList.Shared.Models;

namespace MyList.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Show> Shows { get; set; }
}
