using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PesPlayerSquadNumber.Models;

public class Player
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }

    [MaxLength(200)]
    public string Name { get; init; } = null!;

    [MaxLength(1000)]
    public string ImageUrl { get; init; } = null!;

    [MaxLength(1000)]
    public string Url { get; init; } = null!;

    [MaxLength(100)]
    public string Position { get; init; } = null!;

    [MaxLength(10)]
    public string? Age { get; init; }

    public int? SquadIndex { get; set; }


    public int NationId { get; init; }
    public Nation Nation { get; init; } = null!;

    public int? ClubId { get; init; }
    public Club? Club { get; init; }

    public virtual List<SquadNumber> SquadNumbers { get; init; } = [];

    [NotMapped]
    public int? CurrentSquadNumber => SquadNumbers.Count == 0 ? null : SquadNumbers[^1].Number;

    [NotMapped]
    public int? RecommendedSquadNumber { get; set; }
}