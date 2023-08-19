using System.ComponentModel.DataAnnotations.Schema;

namespace PesPlayerSquadNumber.Models;

public class Player
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string Position { get; set; } = null!;
    public string? Age { get; set; }

    public int? SquadIndex { get; set; }


    public int NationId { get; set; }
    public Nation Nation { get; set; } = null!;

    public int? ClubId { get; set; }
    public Club? Club { get; set; }

    public virtual List<SquadNumber> SquadNumbers { get; set; } = new();

    [NotMapped]
    public int? CurrentSquadNumber => SquadNumbers.Count == 0 ? null : SquadNumbers[^1].Number;

    [NotMapped]
    public int? RecommendedSquadNumber { get; set; }
}