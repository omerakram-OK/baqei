using Microsoft.EntityFrameworkCore;
using Baqei.Domain.Entities;

namespace Baqei.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Plot> Plots => Set<Plot>();
}
