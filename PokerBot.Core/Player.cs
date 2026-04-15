using PokerBot.Helpers;

namespace PokerBot.Core
{
    public class Player
    {
        public string Name { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string Card1AutoId { get; set; } = string.Empty;
        public string Card2AutoId { get; set; } = string.Empty;
        public int Chips { get; set; } = 0;
        public List<Card> HoleCards { get; set; }
        public Player(string number)
        {
            HoleCards = new List<Card>();
            Number = number;
            Card1AutoId = Constants.CardRegex;
            Card1AutoId = Card1AutoId.Replace("?1", Number);
            Card2AutoId = Card1AutoId.Replace("?2", Constants.Card2Id);
            Card1AutoId = Card1AutoId.Replace("?2", Constants.Card1Id);
        }
    }
}
