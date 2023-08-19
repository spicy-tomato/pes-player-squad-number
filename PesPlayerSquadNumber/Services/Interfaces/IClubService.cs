using PesPlayerSquadNumber.Dtos.Transfermarkt;

namespace PesPlayerSquadNumber.Services.Interfaces;

public interface IClubService
{
    public Models.Club? GetByUrl(string? url);

    public void Add(IEnumerable<Club> clubs);
}