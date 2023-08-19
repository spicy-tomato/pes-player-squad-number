using HtmlAgilityPack;
using PesPlayerSquadNumber.Constants;
using PesPlayerSquadNumber.Dtos.Transfermarkt;
using PesPlayerSquadNumber.Models;
using PesPlayerSquadNumber.Services.Interfaces;
using Club = PesPlayerSquadNumber.Dtos.Transfermarkt.Club;
using Nation = PesPlayerSquadNumber.Dtos.Transfermarkt.Nation;

namespace PesPlayerSquadNumber.Services.Implementations;

public class TransfermarktService : ITransfermarktService
{
    private readonly HtmlWeb _web = new();
    private const string BaseUrl = TransfermarktConstant.BaseUrl;

    public List<TransfermarktPlayer> Search(string? playerNameToSearch) => Search(playerNameToSearch, 1);

    public List<TransfermarktPlayer> Search(string? playerNameToSearch, int page)
    {
        if (string.IsNullOrEmpty(playerNameToSearch)) return new List<TransfermarktPlayer>();

        var url = TransfermarktConstant.PlayerList.Url(page, playerNameToSearch);
        const string rootNodeXPath = TransfermarktConstant.PlayerList.RootNodeXPath;
        var nodes = LoadWebAndSelectNodes(url, rootNodeXPath);

        return nodes.Select(ReadPlayerDataFromRowOfList).ToList();
    }

    public List<SquadNumber> GetSquadNumbers(string playerUrl)
    {
        if (string.IsNullOrEmpty(playerUrl)) throw new Exception("URL cannot be null");

        var url = TransfermarktConstant.SquadNumber.Url(playerUrl);
        const string rootNodeXPath = TransfermarktConstant.SquadNumber.RootNodeXPath;
        var squadNumberNodes = LoadWebAndSelectNodes(url, rootNodeXPath);

        var squadNumbersList = squadNumberNodes.Select(ReadSquadNumberHistory).ToList();
        squadNumbersList.Reverse();

        return squadNumbersList;
    }

    private static TransfermarktPlayer ReadPlayerDataFromRowOfList(HtmlNode node)
    {
        Club? club = null;
        try
        {
            // has club
            var clubLinkNode = node.SelectSingleNode(TransfermarktConstant.PlayerList.ClubLinkXPath);
            var clubImageNode = node.SelectSingleNode(TransfermarktConstant.PlayerList.ClubImageXPath);

            var clubUrl = clubLinkNode.GetAttributeValue("href", null);
            var clubName = clubImageNode.GetAttributeValue("title", null);
            var clubImageUrl = clubImageNode.GetAttributeValue("src", null);

            club = new Club(clubName, clubUrl, clubImageUrl);
        }
        catch (Exception)
        {
            // ignored
        }

        var nationNode = node.SelectSingleNode(TransfermarktConstant.PlayerList.NationImageXPath);
        var nameNode = node.SelectSingleNode(TransfermarktConstant.PlayerList.NameXPath);
        var imageNode = node.SelectSingleNode(TransfermarktConstant.PlayerList.ImageXPath);
        var positionNode = node.SelectSingleNode(TransfermarktConstant.PlayerList.PositionXPath);
        var ageNode = node.SelectSingleNode(TransfermarktConstant.PlayerList.AgeXPath);

        var nationName = nationNode?.GetAttributeValue("title", null);
        var nationImageUrl = nationNode?.GetAttributeValue("src", null);

        var playerName = HtmlEntity.DeEntitize(nameNode.InnerText);
        var playerUrl = nameNode.GetAttributeValue("href", null);
        var playerImageUrl = imageNode.GetAttributeValue("src", null);
        var playerPosition = HtmlEntity.DeEntitize(positionNode.InnerText);
        var playerAge = ageNode != null ? HtmlEntity.DeEntitize(ageNode.InnerText) : null;

        var nation = nationName != null && nationImageUrl != null ? new Nation(nationName, nationImageUrl) : null;

        var player = new TransfermarktPlayer(playerName, playerImageUrl, playerUrl, club, playerPosition, playerAge,
            nation);

        return player;
    }

    private static SquadNumber ReadSquadNumberHistory(HtmlNode node)
    {
        var seasonNode = node.SelectSingleNode(TransfermarktConstant.SquadNumber.SeasonXPath);
        var clubImageNode = node.SelectSingleNode(TransfermarktConstant.SquadNumber.ClubImageXPath);
        var clubNameNode = node.SelectSingleNode(TransfermarktConstant.SquadNumber.ClubNameXPath);
        var numberNode = node.SelectSingleNode(TransfermarktConstant.SquadNumber.NumberXPath);

        var season = HtmlEntity.DeEntitize(seasonNode.InnerText);
        var clubImage = clubImageNode.GetAttributeValue("src", null);
        var clubName = HtmlEntity.DeEntitize(clubNameNode.InnerText);
        var number = int.Parse(HtmlEntity.DeEntitize(numberNode.InnerText));

        Models.Club club = new()
        {
            Name = clubName,
            Url = "",
            ImageUrl = clubImage
        };

        SquadNumber squadNumber = new()
        {
            Season = season,
            Number = number,
            Club = club,
        };

        return squadNumber;
    }

    private HtmlNodeCollection LoadWebAndSelectNodes(string url, string rootNodeXPath)
    {
        var document = _web.Load(BaseUrl + url);
        var nodes = SelectNodes(document, rootNodeXPath);
        return nodes;
    }

    private HtmlNodeCollection LoadWebAndSelectNodes(string url, string rootNodeXPath, out HtmlDocument document)
    {
        document = _web.Load(BaseUrl + url);
        var nodes = SelectNodes(document, rootNodeXPath);
        return nodes;
    }

    private static HtmlNodeCollection SelectNodes(HtmlDocument document, string nodeXPath)
    {
        var nodes = document.DocumentNode.SelectNodes(nodeXPath);

        if (nodes == null) throw new Exception($"Cannot scrape data! XPath: {nodeXPath}");

        return nodes;
    }
}