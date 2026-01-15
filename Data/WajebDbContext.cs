using Microsoft.EntityFrameworkCore;
using Wajeb.API.Models;
namespace Wajeb.API.Data;


public class WajebDbContext(DbContextOptions<WajebDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
}
