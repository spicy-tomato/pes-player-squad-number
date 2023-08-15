using System.ComponentModel.DataAnnotations.Schema;

namespace PesPlayerSquadNumber.Models;

public class SquadNumber
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Season { get; set; } = null!;

    public int Number { get; set; }

    public int ClubId { get; set; }
    public Club Club { get; set; } = null!;

    public int PlayerId { get; set; }
    public Player Player { get; set; } = null!;
}