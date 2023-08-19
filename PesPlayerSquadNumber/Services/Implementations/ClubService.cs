using PesPlayerSquadNumber.Dtos.Transfermarkt;
using PesPlayerSquadNumber.Services.Interfaces;

namespace PesPlayerSquadNumber.Services.Implementations;

public class ClubService : IClubService
{
    private readonly ILogger<ClubService> _logger;
    private readonly PpsnDbContext _context;

    public ClubService(ILogger<ClubService> logger, PpsnDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public Models.Club? GetByUrl(string? url) => url == null ? null : _context.Clubs.FirstOrDefault(c => c.Url == url);

    public void Add(IEnumerable<Club> clubs)
    {
        foreach (var club in clubs)
        {
            Add(club);
        }

        _context.SaveChanges();
    }

    private void Add(Club club)
    {
        var clubExists = _context.Clubs.Any(c => c.Url == club.Url);
        if (clubExists) return;

        _context.Clubs.Add(new Models.Club
        {
            Name = club.Name,
            Url = club.Url,
            ImageUrl = club.ImageUrl
        });

        _logger.LogInformation("Added club {ClubName}, {ClubUrl}", club.Name, club.Url);
    }
}