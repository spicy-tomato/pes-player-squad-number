using PesPlayerSquadNumber.Dtos.Transfermarkt;
using PesPlayerSquadNumber.Services.Interfaces;

namespace PesPlayerSquadNumber.Services.Implementations;

public class NationService : INationService
{
    private readonly PpsnDbContext _context;

    public NationService(PpsnDbContext context)
    {
        _context = context;
    }

    public Models.Nation? GetByImageUrl(string? imageUrl) =>
        imageUrl != null ? _context.Nations.FirstOrDefault(c => c.ImageUrl == imageUrl) : null;

    public void Add(IEnumerable<Nation> clubs)
    {
        foreach (Nation club in clubs)
        {
            Add(club);
        }

        _context.SaveChanges();
    }

    private void Add(Nation nation)
    {
        bool nationExists = _context.Nations.Any(c => c.ImageUrl == nation.ImageUrl);
        if (nationExists) return;

        _context.Nations.Add(new Models.Nation
        {
            Name = nation.Name,
            ImageUrl = nation.ImageUrl
        });
    }
}