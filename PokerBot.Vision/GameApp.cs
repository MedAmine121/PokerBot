using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Capturing;
using FlaUI.Core.Definitions;
using FlaUI.Core.WindowsAPI;
using FlaUI.UIA3;
using PokerBot.Core;
using PokerBot.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PokerBot.IO
{
    public class GameApp
    {
        public static Application? app { get; private set; }
        public static string FolderPath { get; private set; } = string.Empty;
        public static IntPtr hwnd { get; private set; }
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public static void init(string processName)
        {
            Process[] processes = Process.GetProcessesByName("pokerth_client");
            if (processes.Length == 0)
            {
                return;
            }
            FolderPath = processes[0].MainModule?.FileName ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(FolderPath))
            {
                int index = FolderPath.LastIndexOf("\\");
                if(index != -1)
                {
                    FolderPath = FolderPath.Substring(0, index);
                    ImageIdentifier.LoadCardTemplates(FolderPath);
                    ImageIdentifier.LoadActionTemplates(FolderPath);

                }
            }
            hwnd = processes[0].MainWindowHandle;
            app = Application.Attach(processes[0].Id);
        }
        public static void ClickButtonByAutomationId(string automationId, int delay = 0)
        {
            if (app == null)
            {
                throw new InvalidOperationException("Application is not attached.");
            }
            using (var automation = new UIA3Automation())
            {
                var window = app.GetMainWindow(automation);
                var button = window.FindFirstDescendant(cf => cf.ByAutomationId(automationId)).AsButton();
                if (button == null)
                {
                    return;
                }
                button.Invoke();
            }
        }
        public static void ClickButtonByName(string name, int delay = 0)
        {
            if (delay > 0)
            {
                Task.Delay(delay).Wait();
            }
            if (app == null)
            {
                throw new InvalidOperationException("Application is not attached.");
            }
            using (var automation = new UIA3Automation())
            {
                var window = app.GetMainWindow(automation);
                var button = window.FindFirstDescendant(cf => cf.ByName(name)).AsButton();
                if (button == null)
                {
                    return;
                }
                button.Invoke();
            }
        }
        public static Bitmap GetImage(string autoId, int divideWidth = 1, int divideHeight = 1)
        {
            if (app == null)
            {
                throw new InvalidOperationException("Application is not attached.");
            }
            using (var automation = new UIA3Automation())
            {
                Window? window = app.GetMainWindow(automation);
                AutomationElement? image = window.FindFirstDescendant(cf => cf.ByAutomationId(autoId));
                if (image == null)
                {
                    throw new InvalidOperationException($"Button with AutomationId '{autoId}' not found.");
                }
                SetForeground();
                Bitmap capturedImage = FlaUI.Core.Capturing.Capture.Element(image).Bitmap;
                Bitmap bmpCrop = new Bitmap(capturedImage.Width / divideWidth, capturedImage.Height / divideHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                using (Graphics g = Graphics.FromImage(bmpCrop))
                {
                    g.DrawImage(capturedImage, new Rectangle(0, 0, bmpCrop.Width, bmpCrop.Height),
                                new Rectangle(0, 0, bmpCrop.Width, bmpCrop.Height), GraphicsUnit.Pixel);
                }
                capturedImage = bmpCrop;
                return capturedImage;
            }
        }
        public static void SetForeground()
        {
            ShowWindow(hwnd, 9);
            SetForegroundWindow(hwnd);
        }
        public static List<Player> GetOpponents()
        {
            if (app == null)
            {
                throw new InvalidOperationException("Application is not attached.");
            }
            using (var automation = new UIA3Automation())
            {
                var window = app.GetMainWindow(automation);
                var group = window.FindFirstDescendant(cf => cf.ByAutomationId(Constants.GroupId));
                if (group == null)
                {
                    return new List<Player>();
                }
                List<Player> players = [];
                foreach(AutomationElement child in group.FindAllChildren())
                {
                    AutomationElement? element = child.FindFirstChild(element => element.ByAutomationId("PlayerName", PropertyConditionFlags.MatchSubstring));
                    if (element != null && element.Name != ""){
                        Match match = Regex.Match(element.AutomationId, "PlayerName(\\d)");
                        if (match.Success)
                        {
                            Player player = new(match.Groups[1].Value);
                            player.HoleCards = GetCards(player);
                            player.Status = GetPlayerStatus(child);
                            players.Add(player);
                        }
                    }
                }
                return players;
            }
        }
        public static List<Card> GetCards(Player player)
        {
            Bitmap card1 = GetImage(player.Card1AutoId, 2);
            Bitmap card2 = GetImage(player.Card2AutoId, 2);
            List<Card> cards = [];
            cards.Add(ImageIdentifier.IdentifyCard(card1));
            cards.Add(ImageIdentifier.IdentifyCard(card2));
            return cards;
        }
        public static Actions? GetPlayerStatus(AutomationElement group)
        {
            AutomationElement? element = group.FindFirstChild(element => element.ByAutomationId("Status", PropertyConditionFlags.MatchSubstring));
            if (element != null)
            {
                var status = GetImage(element.AutomationId);
                return ImageIdentifier.IdentifyStatus(status);
            }
            return null;
        }
    }
}
