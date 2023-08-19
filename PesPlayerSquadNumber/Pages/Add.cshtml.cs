using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PesPlayerSquadNumber.Dtos.Transfermarkt;
using PesPlayerSquadNumber.Services.Interfaces;

namespace PesPlayerSquadNumber.Pages;

public class AddModel : PageModel
{
    private ITransfermarktService _transfermarktService;

    public AddModel(ITransfermarktService transfermarktService)
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