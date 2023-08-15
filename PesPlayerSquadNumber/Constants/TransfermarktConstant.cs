using System.Text.RegularExpressions;

namespace PesPlayerSquadNumber.Constants;

public abstract class TransfermarktConstant
{
    public const string BaseUrl = "https://www.transfermarkt.com";

    public abstract class PlayerList
    {
        public static string Url(int page, string playerName) =>
            $"/schnellsuche/ergebnis/schnellsuche?Spieler_page={page}&ajax=yw0&query={playerName}";

        public const string RootNodeXPath =
            "//*[@id='main']/main/div[1]/div/div/div[1]/div/table/tbody/tr[position()>0]";

        public const string ClubLinkXPath = "td[3]/a";

        public const string ClubImageXPath = "td[3]/a/img";

        public const string NationImageXPath = "td[5]/img";

        public const string NameXPath = "td[1]/table/tr[1]/td[2]/a";

        public const string ImageXPath = "td[1]/table/tr[1]/td[1]/a/img";
        
        public const string PositionXPath = "td[2]";
        
        public const string AgeXPath = "td[4]";
    }

    public abstract class SquadNumber
    {
        public static string Url(string playerUrl) => Regex.Replace(playerUrl, "profil", "rueckennummern");

        public const string RootNodeXPath =
            "//*[@id='main']/main/div[3]/div[1]/div[1]/div/div/table/tbody/tr[position()>0]";
             //html/body/div[3]/main/div[3]/div[1]/div[1]/div/div/table/tbody/tr[1]

        public const string SeasonXPath = "td[1]";
        
        public const string ClubImageXPath = "td[2]/a/img";
        
        public const string ClubNameXPath = "td[3]/a";
        
        public const string NumberXPath = "td[4]";
    }
}