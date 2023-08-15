using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PesPlayerSquadNumber.Dtos.Transfermarkt;
using PesPlayerSquadNumber.Services;

namespace PesPlayerSquadNumber.Pages;

public class AddModel : PageModel
{
    private TransfermarktService _transfermarktService;

    public AddModel(TransfermarktService transfermarktService)
    {
        _transfermarktService = transfermarktService;
    }

    [BindProperty]
    public string? Input { get; set; }

    public List<TransfermarktPlayer> Result { get; set; } = new();

    public void OnGet(string? input = null)
    {
        Result = _transfermarktService.Search(input);

        Input = input;
    }
}