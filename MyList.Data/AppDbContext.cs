using Microsoft.EntityFrameworkCore;
using MyList.Data.Models;

namespace MyList.Data;

/* 
    Bridge b/t C# code and DB 

    DbContext - core base class from EFC 
*/
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Show> Shows { get; set; }
}
