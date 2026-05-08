using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PesPlayerSquadNumber.Models;

public class SquadNumber
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }

    [MaxLength(100)]
    public string Season { get; init; } = null!;

    public int Number { get; init; }

    public int ClubId { get; init; }
    public virtual Club Club { get; init; } = null!;

    public int PlayerId { get; init; }
    public virtual Player Player { get; set; } = null!;
}