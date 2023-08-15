using Microsoft.EntityFrameworkCore;
using PesPlayerSquadNumber.Models;

namespace PesPlayerSquadNumber;

public class PpsnDbContext : DbContext
{
    public PpsnDbContext(DbContextOptions<PpsnDbContext> options)
        : base(options) { }

    public virtual DbSet<Player> Players { get; set; } = null!;
    public virtual DbSet<SquadNumber> SquadNumbers { get; set; } = null!;
    public virtual DbSet<Club> Clubs { get; set; } = null!;
    public virtual DbSet<Nation> Nations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}