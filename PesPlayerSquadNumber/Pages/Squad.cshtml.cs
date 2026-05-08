using Microsoft.AspNetCore.Mvc.RazorPages;
using PesPlayerSquadNumber.Models;
using PesPlayerSquadNumber.Services.Interfaces;

namespace PesPlayerSquadNumber.Pages;

public record NumberPoint(int Number, decimal Point, Player Player, int Index);

public class SquadModel : PageModel
{
    private IPlayerService _playerService;

    public SquadModel(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    public List<Player?> Result { get; set; } = [];

    public void OnGet()
    {
        List<Player?> players = _playerService.GetPlayersInSquad();
        string currentYy = (DateTime.Today.Year % 100).ToString("D2");

        IOrderedEnumerable<NumberPoint> numberPoints = players
            .SelectMany((p, i) => GetPoints(p, i, currentYy))
            .Where(np => np.Point >= 2M)
            .OrderByDescending(np => np.Point)
            .ThenByDescending(np => ParseAge(np.Player.Age));

        HashSet<int> assignedPlayers = [];
        HashSet<int> assignedNumbers = [];

        foreach (NumberPoint numberPoint in numberPoints)
        {
            (int number, _, Player player, int index) = numberPoint;

            if (assignedPlayers.Contains(index) || assignedNumbers.Contains(number))
            {
                continue;
            }

            assignedPlayers.Add(index);
            assignedNumbers.Add(number);
            player.RecommendedSquadNumber = number;
        }

        IEnumerable<Player> unassigned = players
            .OfType<Player>()
            .Where(p => p.RecommendedSquadNumber == null)
            .OrderByDescending(p => ParseAge(p.Age));

        foreach (Player player in unassigned)
        {
            int filler = 2;
            while (assignedNumbers.Contains(filler))
            {
                filler++;
            }

            assignedNumbers.Add(filler);
            player.RecommendedSquadNumber = filler;
        }

        Result = players;
    }

    private static IEnumerable<NumberPoint> GetPoints(Player? p, int index, string currentYy)
    {
        if (p == null) return [];

        Dictionary<int, decimal> numberDict = new();

        foreach (IGrouping<string, SquadNumber> seasonGroup in p.SquadNumbers
                     .Where(sn => sn.Number is > 0 and < 100)
                     .GroupBy(sn => sn.Season))
        {
            List<int> numbers = seasonGroup.Select(sn => sn.Number).Distinct().ToList();
            bool isCurrent = seasonGroup.Key.TrimEnd().EndsWith(currentYy);
            decimal pointPerNumber = (isCurrent ? 10M : 1M) / numbers.Count;

            foreach (int number in numbers)
            {
                decimal oldPoint = numberDict.GetValueOrDefault(number, 0M);
                numberDict[number] = oldPoint + pointPerNumber;
            }
        }

        return numberDict.Select(n => new NumberPoint(n.Key, n.Value, p, index));
    }

    private static int? ParseAge(string? age) => int.TryParse(age, out int parsed) ? parsed : null;
}
