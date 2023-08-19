using Microsoft.AspNetCore.Mvc;
using PesPlayerSquadNumber.Dtos.Transfermarkt;
using PesPlayerSquadNumber.Models;
using PesPlayerSquadNumber.Services.Interfaces;

namespace PesPlayerSquadNumber;

public class PpsnController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public PpsnController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet]
    public List<Player> GetAll()
    {
        return _playerService.GetAll();
    }

    [HttpGet]
    public List<Player?> GetInSquad()
    {
        return _playerService.GetPlayersInSquad();
    }

    [HttpPost]
    public IEnumerable<bool> CheckExists([FromBody] IEnumerable<string> playersUrl)
    {
        return _playerService.CheckExist(playersUrl);
    }

    [HttpPost]
    public bool Add([FromBody] IEnumerable<TransfermarktPlayer> selectedPlayers)
    {
        _playerService.Add(selectedPlayers);
        return true;
    }

    [HttpPost]
    public void AssignToSquad([FromBody] AssignToSquadDto payload)
    {
        _playerService.AssignToSquad(payload.PlayerId, payload.SquadIndex);
    }

    public record AssignToSquadDto(int PlayerId, int SquadIndex);
}