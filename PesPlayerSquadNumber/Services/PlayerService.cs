using Microsoft.EntityFrameworkCore;
using PesPlayerSquadNumber.Dtos.Transfermarkt;
using PesPlayerSquadNumber.Models;
using Club = PesPlayerSquadNumber.Dtos.Transfermarkt.Club;

namespace PesPlayerSquadNumber.Services;

public class PlayerService
{
    private readonly ILogger<PlayerService> _logger;
    private readonly TransfermarktService _transfermarktService;
    private readonly PpsnDbContext _context;
    private readonly ClubService _clubService;
    private readonly NationService _nationService;

    public PlayerService(ILogger<PlayerService> logger,
        TransfermarktService transfermarktService,
        PpsnDbContext context,
        ClubService clubService,
        NationService nationService)
    {
        _logger = logger;
        _transfermarktService = transfermarktService;
        _context = context;
        _clubService = clubService;
        _nationService = nationService;
    }

    public List<Player> GetAll() => _context.Players
        .Include(p => p.Club)
        .Include(p => p.Nation)
        .Include(p => p.SquadNumbers)
        .AsNoTracking()
        .ToList();

    public IEnumerable<bool> CheckExist(IEnumerable<string> playersUrl) => playersUrl.Select(CheckExist);

    private bool CheckExist(string playerUrl) => _context.Players.Any(p => p.Url == playerUrl);

    public void Add(IEnumerable<TransfermarktPlayer> selectedPlayers)
    {
        var transfermarktPlayers = selectedPlayers.ToList();

        _clubService.Add(transfermarktPlayers.Select(p => p.Club)
            .OfType<Club>()
            .DistinctBy(c => c.Url));

        _nationService.Add(transfermarktPlayers.Select(p => p.Nation)
            .DistinctBy(c => c.ImageUrl));

        foreach (var playerDto in transfermarktPlayers)
        {
            var squadNumbers = _transfermarktService.GetSquadNumbers(playerDto.Url);

            var club = _clubService.GetByUrl(playerDto.Club?.Url);
            var nation = _nationService.GetByImageUrl(playerDto.Nation.ImageUrl);
            
            var player = TryAddPlayer(playerDto, club, nation!);

            AddSquadNumbers(player, squadNumbers);
        }

        _context.SaveChanges();
    }

    private Player TryAddPlayer(TransfermarktPlayer playerDto, Models.Club? club, Models.Nation nation)
    {
        var savedPlayer = _context.Players.FirstOrDefault(p => p.Url == playerDto.Url);
        if (savedPlayer != null) return savedPlayer;

        var player = new Player
        {
            Name = playerDto.Name,
            Url = playerDto.Url,
            ImageUrl = playerDto.ImageUrl,
            Age = playerDto.Age,
            Position = playerDto.Position,
            Club = club,
            Nation = nation
        };

        var model = _context.Players.Add(player);
        return model.Entity;
    }

    private void AddSquadNumbers(Player player, List<SquadNumber> squadNumbers)
    {
        foreach (var squadNumber in squadNumbers)
        {
            squadNumber.Player = player;
            _context.SquadNumbers.Add(squadNumber);
            _logger.LogInformation("Added number {SquadNumberNumber} ({SquadNumberSeason}) of {PlayerName}",
                squadNumber.Number, squadNumber.Season, player.Name);
        }
    }
}