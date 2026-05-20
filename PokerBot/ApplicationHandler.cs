using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Capturing;
using FlaUI.UIA3;
using PokerBot.Core;
using PokerBot.Helpers;
using PokerBot.IO;
using System.Diagnostics;
using System.Drawing;

namespace PokerBot.App
{
    internal class ApplicationHandler
    {
        public static void StartGame()
        {
            GameApp.ClickButtonByAutomationId(Constants.StartLocalGameAutoId);
            GameApp.ClickButtonByName(Constants.OKName, 500);
            PlayGame();

        }
        public static void PlayGame()
        {
           while(GameState.CurrentState == GameState.State.Playing)
            {
                Task.Delay(100).Wait();
                GetGameState();

            }
        }
        public static void GetGameState()
        {
            GetMyCards();
            GetOpponents();
        }
        public static void GetMyCards()
        {
            GameState.Me.HoleCards = OutputHandler.GetCards(GameState.Me);
        }
        public static void GetOpponents()
        {
            GameState.Others = OutputHandler.GetOthersStatus();
        }
    }
}
