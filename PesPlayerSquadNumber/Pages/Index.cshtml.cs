using Microsoft.AspNetCore.Mvc.RazorPages;
using PesPlayerSquadNumber.Models;
using PesPlayerSquadNumber.Services;

namespace PesPlayerSquadNumber.Pages;

public class IndexModel : PageModel
{
    private PlayerService _playerService;

    public IndexModel(PlayerService playerService)
    {
        _playerService = playerService;
    }
    
    public List<Player> Result { get; set; } = new();

    public void OnGet()
    {
        Result = _playerService.GetAll();
    }
}