using PesPlayerSquadNumber.Dtos.Transfermarkt;

namespace PesPlayerSquadNumber.Services;

public class NationService
{
    private readonly ILogger<NationService> _logger;
    private readonly PpsnDbContext _context;

    public NationService(ILogger<NationService> logger, PpsnDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public Models.Nation? GetByImageUrl(string imageUrl) => _context.Nations.FirstOrDefault(c => c.ImageUrl == imageUrl);

    public void Add(IEnumerable<Nation> clubs)
    {
        foreach (var club in clubs)
        {
            Add(club);
        }

        _context.SaveChanges();
    }

    private void Add(Nation club)
    {
        var clubExists = _context.Nations.Any(c => c.ImageUrl == club.ImageUrl);
        if (clubExists) return;

        _context.Nations.Add(new Models.Nation
        {
            Name = club.Name,
            ImageUrl = club.ImageUrl
        });
    }
}