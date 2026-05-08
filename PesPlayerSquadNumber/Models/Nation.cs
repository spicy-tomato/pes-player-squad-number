using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PesPlayerSquadNumber.Models;

public class Nation
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }

    [MaxLength(200)]
    public string Name { get; init; } = null!;

    [MaxLength(1000)]
    public string ImageUrl { get; init; } = null!;

    public virtual List<Player> Players { get; init; } = [];
}