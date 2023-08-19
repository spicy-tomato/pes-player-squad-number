using Microsoft.AspNetCore.Mvc.RazorPages;
using PesPlayerSquadNumber.Models;
using PesPlayerSquadNumber.Services.Interfaces;

namespace PesPlayerSquadNumber.Pages;

public class IndexModel : PageModel
{
    private IPlayerService _playerService;

    public IndexModel(IPlayerService playerService)
    {
        _playerService = playerService;
    }
    
    public List<Player> Result { get; set; } = new();

    public void OnGet()
    {
        Result = _playerService.GetAll();
    }
}