using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PesPlayerSquadNumber.Models;

public class Club
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }

    [MaxLength(500)]
    public string Name { get; init; } = null!;

    [MaxLength(1000)]
    public string Url { get; init; } = null!;

    [MaxLength(1000)]
    public string ImageUrl { get; init; } = null!;

    public virtual List<Player> Players { get; init; } = [];

    public virtual List<SquadNumber> SquadNumbers { get; init; } = [];
}