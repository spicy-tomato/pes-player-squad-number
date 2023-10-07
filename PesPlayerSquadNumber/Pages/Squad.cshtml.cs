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

    public List<Player?> Result { get; set; } = new();

    public void OnGet()
    {
        var players = _playerService.GetPlayersInSquad();
        var numberPoints = players.SelectMany(GetPoints)
            .OrderByDescending(np => np.Point)
            .ThenByDescending(np => np.Player.Age);
        HashSet<int> assignedPlayers = new();
        HashSet<int> assignedNumbers = new();

        foreach (var numberPoint in numberPoints)
        {
            var (number, _, player, index) = numberPoint;

            if (assignedPlayers.Contains(index) || assignedNumbers.Contains(number))
            {
                continue;
            }

            assignedPlayers.Add(index);
            assignedNumbers.Add(number);
            player.RecommendedSquadNumber = number;
        }

        foreach (var player in players)
        {
            if (player is not { RecommendedSquadNumber: null }) continue;

            var randomNumber = 2;
            while (assignedNumbers.Contains(randomNumber))
            {
                randomNumber++;
            }

            assignedNumbers.Add(randomNumber);
            player.RecommendedSquadNumber = randomNumber;
        }

        Result = players;
    }

    private static IEnumerable<NumberPoint> GetPoints(Player? p, int index)
    {
        if (p == null) return Array.Empty<NumberPoint>();

        var numbersInYears = p.SquadNumbers
            .Where(sn => sn.Number < 100)
            .OrderByDescending(sn => sn.Season)
            .ThenBy(sn => sn.Number)
            .GroupBy(sn => sn.Season)
            .Select(sn => sn.Select(x => x.Number).ToList())
            .ToList();

        Dictionary<int, decimal> numberDict = new();

        for (var i = 0; i < numbersInYears.Count; i++)
        {
            var numbers = numbersInYears[i];
            for (var j = 0; j < numbers.Count; j++)
            {
                var number = numbers[j];
                if (i == 0 && j == 0)
                {
                    numberDict.Add(number, 10);
                    continue;
                }

                var oldPoint = numberDict.GetValueOrDefault(number, 0);
                var newPoint = oldPoint + 1M / numbers.Count;
                numberDict[number] = newPoint;
            }
        }

        return numberDict.Select(n => new NumberPoint(n.Key, n.Value, p, index));
    }
}