using PesPlayerSquadNumber.Dtos.Transfermarkt;
using PesPlayerSquadNumber.Services.Interfaces;

namespace PesPlayerSquadNumber.Services.Implementations;

public class NationService : INationService
{
    private readonly ILogger<NationService> _logger;
    private readonly PpsnDbContext _context;

    public NationService(ILogger<NationService> logger, PpsnDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public Models.Nation? GetByImageUrl(string? imageUrl) =>
        imageUrl != null ? _context.Nations.FirstOrDefault(c => c.ImageUrl == imageUrl) : null;

    public void Add(IEnumerable<Nation> clubs)
    {
        foreach (var club in clubs)
        {
            Add(club);
        }

        _context.SaveChanges();
    }

    private void Add(Nation nation)
    {
        var nationExists = _context.Nations.Any(c => c.ImageUrl == nation.ImageUrl);
        if (nationExists) return;

        _context.Nations.Add(new Models.Nation
        {
            Name = nation.Name,
            ImageUrl = nation.ImageUrl
        });
    }
}