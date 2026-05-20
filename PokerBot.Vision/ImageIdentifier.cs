using AForge.Imaging;
using PokerBot.Core;
using PokerBot.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace PokerBot.IO
{
    public class ImageIdentifier
    {
        public static Dictionary<Card, Bitmap> CardTemplates { get; private set; } = new Dictionary<Card, Bitmap>();
        public static Dictionary<Actions, Bitmap> ActionTemplates { get; private set; } = new Dictionary<Actions, Bitmap>();
        public ImageIdentifier() { }
        public static Card IdentifyCard(Bitmap card)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0.95f);

            foreach(var template in CardTemplates){
                TemplateMatch[] matches = tm.ProcessImage(template.Value, card);

                if (matches.Length > 0)
                {
                    return template.Key;
                }
            }
            return new Card();
        }
        public static Actions? IdentifyStatus(Bitmap status)
        {
            try
            {
                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                {
                    using (var page = engine.Process(status))
                    {
                        var text = page.GetText();
                        Console.WriteLine("Mean confidence: {0}", page.GetMeanConfidence());

                        Console.WriteLine("Text (GetText): \r\n{0}", text);
                        Console.WriteLine("Text (iterator):");
                        using (var iter = page.GetIterator())
                        {
                            iter.Begin();

                            do
                            {
                                do
                                {
                                    do
                                    {
                                        do
                                        {
                                            if (iter.IsAtBeginningOf(PageIteratorLevel.Block))
                                            {
                                                Console.WriteLine("<BLOCK>");
                                            }

                                            Console.Write(iter.GetText(PageIteratorLevel.Word));
                                            Console.Write(" ");

                                            if (iter.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word))
                                            {
                                                Console.WriteLine();
                                            }
                                        } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));

                                        if (iter.IsAtFinalOf(PageIteratorLevel.Para, PageIteratorLevel.TextLine))
                                        {
                                            Console.WriteLine();
                                        }
                                    } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                                } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
                            } while (iter.Next(PageIteratorLevel.Block));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                Console.WriteLine("Unexpected Error: " + e.Message);
                Console.WriteLine("Details: ");
                Console.WriteLine(e.ToString());
            }
            return null;
        }
        public static void LoadCardTemplates(string FolderPath)
        {
            string[] files = Directory.GetFiles(FolderPath + "\\" + Constants.CardFilesPath, "*.png");
            foreach (string file in files) {
                string fileName = Path.GetFileNameWithoutExtension(file);
                if (fileName.StartsWith(Constants.FlipsideFileName))
                {
                    Bitmap flipsideImage = new(file);
                    Card card = new Card(Constants.FlipsideFileName, Constants.FlipsideFileName);
                    flipsideImage = ConvertToAForgeFormat(flipsideImage);
                    CardTemplates[card] = flipsideImage;
                }
                else
                {
                    Bitmap cardTemplate = new(file);
                    cardTemplate = ConvertToAForgeFormat(cardTemplate);
                    Card? card = GetTemplateDetails(fileName);
                    if(card != null)
                    {
                        CardTemplates[card] = cardTemplate;
                    }
                }
            }
        }
        public static void LoadActionTemplates(string FolderPath)
        {
            string[] files = Directory.GetFiles(FolderPath + "\\" + Constants.ActionFilesPath, "*.png");
            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                Bitmap actionImage = new(file);
                actionImage = ConvertToAForgeFormat(actionImage);
                foreach(Actions action in Enum.GetValues<Actions>())
                {
                    if (fileName.Replace("action_","") == action.ToString().ToLower())
                    {
                        ActionTemplates[action] = actionImage;
                        break;
                    }
                }
            }
        }
        public static Bitmap ConvertToAForgeFormat(Bitmap original)
        {
            Bitmap clone = new Bitmap(original.Width, original.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            using (Graphics gr = Graphics.FromImage(clone))
            {
                gr.DrawImage(original, new Rectangle(0, 0, clone.Width, clone.Height));
            }

            return clone;
        }
        public static Card? GetTemplateDetails(string fileName)
        {
            string suit = string.Empty;
            if(string.Compare(fileName, Constants.DiamondRange.Item1) >= 0 && string.Compare(fileName,Constants.DiamondRange.Item2) <= 0)
            {
                suit = Constants.Diamonds;
            }
            else if (string.Compare(fileName, Constants.HeartsRange.Item1) >= 0 && string.Compare(fileName, Constants.HeartsRange.Item2) <= 0)
            {
                suit = Constants.Hearts;
            }
            else if (string.Compare(fileName, Constants.SpadesRange.Item1) >= 0 && string.Compare(fileName, Constants.SpadesRange.Item2) <= 0)
            {
                suit = Constants.Spades;
            }
            else if (string.Compare(fileName, Constants.ClubsRange.Item1) >= 0 && string.Compare(fileName, Constants.ClubsRange.Item2) <= 0)
            {
                suit = Constants.Clubs;
            }
            else
            {
                return null;
            }
            int rank = Convert.ToInt32(fileName) % 13;
            string cardRank = string.Empty;
            if(rank < 9)
            {
                cardRank = (rank + 2).ToString();
            }
            else if(rank == 9)
            {
                cardRank = "J";
            }
            else if(rank == 10)
            {
                cardRank = "Q";
            }
            else if(rank == 11)
            {
                cardRank = "K";
            }
            else if(rank == 12)
            {
                cardRank = "Ace";
            }
            return new Card(suit, cardRank);
        }
    }
}
