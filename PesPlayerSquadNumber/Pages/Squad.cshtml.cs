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
        IOrderedEnumerable<NumberPoint> numberPoints = players.SelectMany(GetPoints)
            .OrderByDescending(np => np.Point)
            .ThenByDescending(np => np.Player.Age);
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

        foreach (Player? player in players)
        {
            if (player is not { RecommendedSquadNumber: null }) continue;

            int randomNumber = 2;
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
        if (p == null) return [];

        List<List<int>> numbersInYears = p.SquadNumbers
            .Where(sn => sn.Number < 100)
            .OrderByDescending(sn => sn.Season)
            .ThenBy(sn => sn.Number)
            .GroupBy(sn => sn.Season)
            .Select(sn => sn.Select(x => x.Number).ToList())
            .ToList();

        Dictionary<int, decimal> numberDict = new();

        for (int i = 0; i < numbersInYears.Count; i++)
        {
            List<int> numbers = numbersInYears[i];
            for (int j = 0; j < numbers.Count; j++)
            {
                int number = numbers[j];
                if (i == 0 && j == 0)
                {
                    numberDict.Add(number, 10);
                    continue;
                }

                decimal oldPoint = numberDict.GetValueOrDefault(number, 0);
                decimal newPoint = oldPoint + 1M / numbers.Count;
                numberDict[number] = newPoint;
            }
        }

        return numberDict.Select(n => new NumberPoint(n.Key, n.Value, p, index));
    }
}