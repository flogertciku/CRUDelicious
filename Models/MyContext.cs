#pragma warning disable CS8618
using CRUDelicious.Models;
using Microsoft.EntityFrameworkCore;
namespace CRUDelicious.Models;
public class MyContext : DbContext 
{    
    public MyContext(DbContextOptions options) : base(options) { }    
      public DbSet<Dish> Dishes { get; set; } 
      public DbSet<User> Users {get;set;}
}
