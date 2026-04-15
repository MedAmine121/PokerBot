using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerBot.Helpers
{
    public class Constants
    {
        public const string StartLocalGameAutoId = "QApplication.startWindow.centralwidget.pushButtonStart_Local_Game";
        public const string OKName = "OK";
        public const string CardRegex = "QApplication.gameTable.centralwidget.groupBox?1.pixmapLabel_card?1?2";
        public const string Card1Id = "a";
        public const string Card2Id = "b";
        public const string FlipsideFileName = "flipside";
        public static readonly (string, string) DiamondRange = ("0", "12");
        public static readonly (string, string) HeartsRange = ("13", "25");
        public static readonly (string, string) SpadesRange = ("26", "38");
        public static readonly (string, string) ClubsRange = ("39", "51");
        public const string Diamonds = "Diamonds";
        public const string Hearts = "Hearts";
        public const string Spades = "Spades";
        public const string Clubs = "Clubs";
        public const string CardFilesPath = "data\\gfx\\cards\\default_800x480";
    }
}
