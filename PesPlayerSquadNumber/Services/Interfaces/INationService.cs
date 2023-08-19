using PesPlayerSquadNumber.Dtos.Transfermarkt;

namespace PesPlayerSquadNumber.Services.Interfaces;

public interface INationService
{
    public Models.Nation? GetByImageUrl(string? imageUrl);

    public void Add(IEnumerable<Nation> clubs);
}