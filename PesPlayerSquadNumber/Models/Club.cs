using System.ComponentModel.DataAnnotations.Schema;

namespace PesPlayerSquadNumber.Models;

public class Club
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;

    public virtual List<Player> Players { get; set; } = new();

    public virtual List<SquadNumber> SquadNumbers { get; set; } = new();
}