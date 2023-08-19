using PesPlayerSquadNumber.Dtos.Transfermarkt;
using PesPlayerSquadNumber.Models;

namespace PesPlayerSquadNumber.Services.Interfaces;

public interface IPlayerService
{
    public List<Player> GetAll();

    public List<Player?> GetPlayersInSquad();

    public IEnumerable<bool> CheckExist(IEnumerable<string> playersUrl);

    public void Add(IEnumerable<TransfermarktPlayer> selectedPlayers);

    public void AssignToSquad(int playerId, int squadIndex);
}