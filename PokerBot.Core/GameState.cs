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
        public static float PotSize { get; set; } = 0;
        public static float TotalBets { get; set; } = 0;
        public static int HandNumber { get; set; } = 0;
        public static int GameNumber { get; set; } = 0;
        public static List<Player> Others { get; set; } = new List<Player>();
        public static State CurrentState { get; set; } = State.Playing;
        public static BoardState CurrentBoardState { get; set; } = BoardState.Preflop;
    }
    public enum BoardState
    {
        Preflop,
        Flop,
        Turn,
        River,
        Ended
    }
    public enum State
    {
        Waiting,
        Playing,
        Ended
    }
}
