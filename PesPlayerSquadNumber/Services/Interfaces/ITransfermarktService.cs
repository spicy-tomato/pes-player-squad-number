using PesPlayerSquadNumber.Dtos.Transfermarkt;
using PesPlayerSquadNumber.Models;

namespace PesPlayerSquadNumber.Services.Interfaces;

public interface ITransfermarktService
{
    public List<TransfermarktPlayer> Search(string? playerNameToSearch);

    public List<TransfermarktPlayer> Search(string? playerNameToSearch, int page);

    public List<SquadNumber> GetSquadNumbers(string playerUrl);
}