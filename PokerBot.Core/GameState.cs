using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerBot.Core
{
    public static class GameState
    {
        public static Player Me { get; set; } = new Player("0");
        public static int PotSize { get; set; } = 0;
        public static List<Player> Others { get; set; } = new List<Player>();
        public enum State
        {
            Waiting,
            Playing,
            Ended
        }
        public static State CurrentState { get; set; } = State.Playing;
    }
}
