using System.ComponentModel.DataAnnotations.Schema;

namespace PesPlayerSquadNumber.Models;

public class Nation
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;

    public List<Player> Players { get; set; } = new();
}