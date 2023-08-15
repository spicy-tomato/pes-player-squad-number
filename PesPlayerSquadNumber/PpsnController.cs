using Microsoft.AspNetCore.Mvc;
using PesPlayerSquadNumber.Dtos.Transfermarkt;
using PesPlayerSquadNumber.Models;
using PesPlayerSquadNumber.Services;

namespace PesPlayerSquadNumber;

public class PpsnController : ControllerBase
{
    private readonly PlayerService _playerService;

    public PpsnController(PlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet]
    public List<Player> GetAll()
    {
        return _playerService.GetAll().ToList();
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
}