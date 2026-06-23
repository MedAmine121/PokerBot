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
        public const string GroupId = "QApplication.gameTable.centralwidget";
        public const string Card1Id = "a";
        public const string Card2Id = "b";
        public const string PotId = "QApplication.gameTable.centralwidget.groupBox_Board.widget_2.textLabel_Pot";
        public const string TotalBetsId = "QApplication.gameTable.centralwidget.groupBox_Board.widget_2.textLabel_Sets";
        public const string GameNumberId = "QApplication.gameTable.centralwidget.groupBox_Board.framegameinfo.label_gameNumberValue";
        public const string HandNumberId = "QApplication.gameTable.centralwidget.groupBox_Board.framegameinfo.label_handNumberValue";
        public const string BoardStateId = "QApplication.gameTable.centralwidget.groupBox_Board.textLabel_handLabel";
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
        public const string ActionFilesPath = "data\\gfx\\gui\\misc\\actionpics";
    }
    public enum Actions
    {
        Allin,
        Bet,
        Call,
        Check,
        Fold,
        Raise,
        Winner
    }
}
