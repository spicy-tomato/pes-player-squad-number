using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PesPlayerSquadNumber.Dtos.Transfermarkt;
using PesPlayerSquadNumber.Models;
using PesPlayerSquadNumber.Services.Interfaces;
using Club = PesPlayerSquadNumber.Dtos.Transfermarkt.Club;
using Nation = PesPlayerSquadNumber.Dtos.Transfermarkt.Nation;

namespace PesPlayerSquadNumber.Services.Implementations;

public class PlayerService : IPlayerService
{
    private readonly ILogger<PlayerService> _logger;
    private readonly PpsnDbContext _context;
    private readonly ITransfermarktService _transfermarktService;
    private readonly IClubService _clubService;
    private readonly INationService _nationService;

    public PlayerService(ILogger<PlayerService> logger, 
        PpsnDbContext context,
        ITransfermarktService transfermarktService,
        IClubService clubService,
        INationService nationService)
    {
        _logger = logger;
        _context = context;
        _transfermarktService = transfermarktService;
        _clubService = clubService;
        _nationService = nationService;
    }

    public List<Player> GetAll() => _context.Players
        .Include(p => p.Club)
        .Include(p => p.Nation)
        .Include(p => p.SquadNumbers)
        .AsNoTracking()
        .ToList();

    public List<Player?> GetPlayersInSquad()
    {
        List<Player?> players = Enumerable.Repeat<Player?>(null, 23).ToList();

        List<Player> playersInSquad = _context.Players
            .Include(p => p.Club)
            .Include(p => p.Nation)
            .Include(p => p.SquadNumbers)
            .Where(p => p.SquadIndex != null)
            .AsNoTracking()
            .ToList();

        foreach (Player player in playersInSquad)
        {
            players[player.SquadIndex!.Value] = player;
        }

        return players;
    }

    public IEnumerable<bool> CheckExist(IEnumerable<string> playersUrl) => playersUrl.Select(CheckExist);

    private bool CheckExist(string playerUrl) => _context.Players.Any(p => p.Url == playerUrl);

    public void Add(IEnumerable<TransfermarktPlayer> selectedPlayers)
    {
        List<TransfermarktPlayer> transfermarktPlayers = selectedPlayers.ToList();

        _clubService.Add(transfermarktPlayers.Select(p => p.Club)
            .OfType<Club>()
            .DistinctBy(c => c.Url));

        _nationService.Add(transfermarktPlayers.Select(p => p.Nation)
            .OfType<Nation>()
            .DistinctBy(c => c.ImageUrl));

        foreach (TransfermarktPlayer playerDto in transfermarktPlayers)
        {
            List<SquadNumber> squadNumbers = _transfermarktService.GetSquadNumbers(playerDto.Url);

            Models.Club? club = _clubService.GetByUrl(playerDto.Club?.Url);
            Models.Nation? nation = _nationService.GetByImageUrl(playerDto.Nation?.ImageUrl);

            Player player = TryAddPlayer(playerDto, club, nation!);

            AddSquadNumbers(player, squadNumbers);
        }

        _context.SaveChanges();
    }

    public void AssignToSquad(int playerId, int squadIndex)
    {
        Player? player = _context.Players.FirstOrDefault(p => p.Id == playerId);
        if (player == null) throw new Exception($"Cannot find player with ID {playerId}");

        Player? assignedPlayer = _context.Players.FirstOrDefault(p => p.SquadIndex == squadIndex);
        assignedPlayer?.SquadIndex = player.SquadIndex;

        player.SquadIndex = squadIndex;

        _context.SaveChanges();
    }

    private Player TryAddPlayer(TransfermarktPlayer playerDto, Models.Club? club, Models.Nation nation)
    {
        Player? savedPlayer = _context.Players.FirstOrDefault(p => p.Url == playerDto.Url);
        if (savedPlayer != null) return savedPlayer;

        Player player = new()
        {
            Name = playerDto.Name,
            Url = playerDto.Url,
            ImageUrl = playerDto.ImageUrl,
            Age = playerDto.Age,
            Position = playerDto.Position,
            Club = club,
            Nation = nation
        };

        EntityEntry<Player> model = _context.Players.Add(player);
        return model.Entity;
    }

    private void AddSquadNumbers(Player player, List<SquadNumber> squadNumbers)
    {
        foreach (SquadNumber squadNumber in squadNumbers)
        {
            squadNumber.Player = player;
            _context.SquadNumbers.Add(squadNumber);
            _logger.LogInformation("Added number {SquadNumberNumber} ({SquadNumberSeason}) of {PlayerName}",
                squadNumber.Number, squadNumber.Season, player.Name);
        }
    }
}