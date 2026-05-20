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
        public static List<Card> GetCards(Player player)
        {
            return GameApp.GetCards(player);
        }
        public static List<Player> GetOthersStatus()
        {
            return GameApp.GetOpponents();
        }
    }
}
