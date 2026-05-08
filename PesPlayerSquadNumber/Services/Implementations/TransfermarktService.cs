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
        if (string.IsNullOrEmpty(playerNameToSearch)) return [];

        string url = TransfermarktConstant.PlayerList.Url(page, playerNameToSearch);
        const string rootNodeXPath = TransfermarktConstant.PlayerList.RootNodeXPath;
        HtmlNodeCollection nodes = LoadWebAndSelectNodes(url, rootNodeXPath);

        return nodes.Select(ReadPlayerDataFromRowOfList).ToList();
    }

    public List<SquadNumber> GetSquadNumbers(string playerUrl)
    {
        if (string.IsNullOrEmpty(playerUrl)) throw new Exception("URL cannot be null");

        string url = TransfermarktConstant.SquadNumber.Url(playerUrl);
        const string rootNodeXPath = TransfermarktConstant.SquadNumber.RootNodeXPath;
        HtmlNodeCollection squadNumberNodes = LoadWebAndSelectNodes(url, rootNodeXPath);

        List<SquadNumber> squadNumbersList = squadNumberNodes.Select(ReadSquadNumberHistory).ToList();
        squadNumbersList.Reverse();

        return squadNumbersList;
    }

    private static TransfermarktPlayer ReadPlayerDataFromRowOfList(HtmlNode node)
    {
        Club? club = null;
        try
        {
            // has club
            HtmlNode clubLinkNode = node.SelectSingleNode(TransfermarktConstant.PlayerList.ClubLinkXPath);
            HtmlNode clubImageNode = node.SelectSingleNode(TransfermarktConstant.PlayerList.ClubImageXPath);

            string clubUrl = clubLinkNode.GetAttributeValue("href", string.Empty);
            string clubName = clubImageNode.GetAttributeValue("title", string.Empty);
            string clubImageUrl = clubImageNode.GetAttributeValue("src", string.Empty);

            club = new Club(clubName, clubUrl, clubImageUrl);
        }
        catch (Exception)
        {
            // ignored
        }

        HtmlNode nationNode = node.SelectSingleNode(TransfermarktConstant.PlayerList.NationImageXPath);
        HtmlNode nameNode = node.SelectSingleNode(TransfermarktConstant.PlayerList.NameXPath);
        HtmlNode imageNode = node.SelectSingleNode(TransfermarktConstant.PlayerList.ImageXPath);
        HtmlNode positionNode = node.SelectSingleNode(TransfermarktConstant.PlayerList.PositionXPath);
        HtmlNode ageNode = node.SelectSingleNode(TransfermarktConstant.PlayerList.AgeXPath);

        string nationName = nationNode.GetAttributeValue("title", string.Empty);
        string nationImageUrl = nationNode.GetAttributeValue("src", string.Empty);

        string playerName = HtmlEntity.DeEntitize(nameNode.InnerText);
        string playerUrl = nameNode.GetAttributeValue("href", string.Empty);
        string playerImageUrl = imageNode.GetAttributeValue("src", string.Empty);
        string playerPosition = HtmlEntity.DeEntitize(positionNode.InnerText);
        string playerAge = HtmlEntity.DeEntitize(ageNode.InnerText);

        Nation nation = new(nationName, nationImageUrl);

        TransfermarktPlayer player = new(playerName, playerImageUrl, playerUrl, club, playerPosition, playerAge,
            nation);

        return player;
    }

    private static SquadNumber ReadSquadNumberHistory(HtmlNode node)
    {
        HtmlNode seasonNode = node.SelectSingleNode(TransfermarktConstant.SquadNumber.SeasonXPath);
        HtmlNode clubImageNode = node.SelectSingleNode(TransfermarktConstant.SquadNumber.ClubImageXPath);
        HtmlNode clubNameNode = node.SelectSingleNode(TransfermarktConstant.SquadNumber.ClubNameXPath);
        HtmlNode numberNode = node.SelectSingleNode(TransfermarktConstant.SquadNumber.NumberXPath);

        string season = HtmlEntity.DeEntitize(seasonNode.InnerText);
        string clubImage = clubImageNode.GetAttributeValue("src", string.Empty);
        string clubName = HtmlEntity.DeEntitize(clubNameNode.InnerText);
        int number = int.Parse(HtmlEntity.DeEntitize(numberNode.InnerText));

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
        HtmlDocument document = _web.Load(BaseUrl + url);
        HtmlNodeCollection nodes = SelectNodes(document, rootNodeXPath);
        return nodes;
    }

    private static HtmlNodeCollection SelectNodes(HtmlDocument document, string nodeXPath)
    {
        HtmlNodeCollection? nodes = document.DocumentNode.SelectNodes(nodeXPath);

        return nodes ?? throw new Exception($"Cannot scrape data! XPath: {nodeXPath}");
    }
}