using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using PokerBot.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerBot.IO
{
    public class OutputHandler
    {
        public static Player GetCards(Player player)
        {
            Bitmap card1 = GameApp.GetImage(player.Card1AutoId, 2);
            Bitmap card2 = GameApp.GetImage(player.Card2AutoId, 2);
            player.HoleCards.Add(ImageIdentifier.IdentifyCard(card1));
            player.HoleCards.Add(ImageIdentifier.IdentifyCard(card2));
            return player;
        }
    }
}
