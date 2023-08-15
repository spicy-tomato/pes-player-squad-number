namespace PesPlayerSquadNumber.Dtos.Transfermarkt;

public record TransfermarktPlayer(string Name,
    string ImageUrl,
    string Url,
    Club? Club,
    string Position,
    string? Age,
    Nation? Nation);