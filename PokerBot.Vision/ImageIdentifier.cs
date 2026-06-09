using AForge.Imaging;
using OpenCvSharp;
using OpenCvSharp.Extensions;
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
        public static Dictionary<Card, Mat> CardTemplates { get; private set; } = new Dictionary<Card, Mat>();
        public static Dictionary<Actions, Mat> ActionTemplates { get; private set; } = new Dictionary<Actions, Mat>();
        public ImageIdentifier() { }
        public static Card IdentifyCard(Bitmap card)
        {
            Mat capturedCardScene = BitmapConverter.ToMat(card);
            double threshold = 0.80;
            Card cardResult = new();
            double bestSimilarity = 0;
            using Mat sceneBGR = ToBgr(capturedCardScene);

            foreach (var template in CardTemplates)
            {
                Mat templateMat = template.Value;

                using Mat templateBGR = ToBgr(templateMat);

                if (templateBGR.Width < sceneBGR.Width || templateBGR.Height < sceneBGR.Height)
                    continue;

                int resultWidth = templateBGR.Width - sceneBGR.Width + 1;
                int resultHeight = templateBGR.Height - sceneBGR.Height + 1;
                using Mat result = new Mat(resultHeight, resultWidth, MatType.CV_32FC1);


                Cv2.MatchTemplate(templateBGR, sceneBGR, result, TemplateMatchModes.CCoeffNormed);

                Cv2.MinMaxLoc(result, out double minVal, out double maxVal, out OpenCvSharp.Point minLoc, out OpenCvSharp.Point maxLoc);

                double currentSimilarity = maxVal;

                if (currentSimilarity >= threshold && currentSimilarity > bestSimilarity)
                {
                    cardResult = template.Key;
                    bestSimilarity = currentSimilarity;
                }
            }

            return cardResult;
        }
        public static Actions? IdentifyStatus(Bitmap status)
        {
            Mat capturedStatusScene = BitmapConverter.ToMat(status);
            double threshold = 0.95;

            using Mat sceneBGR = new Mat();
            if (capturedStatusScene.Channels() == 4)
                Cv2.CvtColor(capturedStatusScene, sceneBGR, ColorConversionCodes.BGRA2BGR);
            else
                capturedStatusScene.CopyTo(sceneBGR);

            foreach (var template in ActionTemplates)
            {
                Mat templateMat = template.Value;

                using Mat templateBGR = new Mat();
                Mat alphaMask = new Mat();

                if (templateMat.Channels() == 4)
                {
                    Mat[] channels = Cv2.Split(templateMat);

                    Cv2.Merge(new Mat[] { channels[0], channels[1], channels[2] }, templateBGR);

                    alphaMask = channels[3];

                }
                else
                {
                    templateMat.CopyTo(templateBGR);
                }

                if (templateBGR.Width > sceneBGR.Width || templateBGR.Height > sceneBGR.Height)
                    continue;

                int resultWidth = sceneBGR.Width - templateBGR.Width + 1;
                int resultHeight = sceneBGR.Height - templateBGR.Height + 1;
                using Mat result = new Mat(resultHeight, resultWidth, MatType.CV_32FC1);

                if (alphaMask.Empty())
                {
                    Cv2.MatchTemplate(sceneBGR, templateBGR, result, TemplateMatchModes.SqDiffNormed);
                }
                else
                {
                    Cv2.MatchTemplate(sceneBGR, templateBGR, result, TemplateMatchModes.SqDiffNormed, alphaMask);
                }

                Cv2.MinMaxLoc(result, out double minVal, out double maxVal, out OpenCvSharp.Point minLoc, out OpenCvSharp.Point maxLoc);

                double currentSimilarity = 1.0 - minVal;

                if (currentSimilarity >= threshold)
                {
                    return template.Key;
                }
            }
            return null;
        }
        public static void LoadCardTemplates(string FolderPath)
        {
            foreach (var kvp in CardTemplates) kvp.Value?.Dispose();
            CardTemplates.Clear();

            string targetDirectory = Path.Combine(FolderPath, Constants.CardFilesPath);
            if (!Directory.Exists(targetDirectory)) return;

            string[] files = Directory.GetFiles(targetDirectory, "*.png");

            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);

                Mat cardTemplateMat = Cv2.ImRead(file, ImreadModes.Unchanged);

                if (cardTemplateMat.Empty())
                {
                    cardTemplateMat.Dispose();
                    continue;
                }

                if (fileName.StartsWith(Constants.FlipsideFileName))
                {
                    Card card = new Card(Constants.FlipsideFileName, Constants.FlipsideFileName);
                    CardTemplates[card] = cardTemplateMat;
                }
                else
                {
                    Card? card = GetTemplateDetails(fileName);
                    if (card != null)
                    {
                        CardTemplates[card] = cardTemplateMat;
                    }
                    else
                    {
                        cardTemplateMat.Dispose();
                    }
                }
            }
        }
        public static void LoadActionTemplates(string FolderPath)
        {
            foreach (var kvp in ActionTemplates) kvp.Value?.Dispose();
            ActionTemplates.Clear();

            string targetDirectory = Path.Combine(FolderPath, Constants.ActionFilesPath);
            if (!Directory.Exists(targetDirectory)) return;

            string[] files = Directory.GetFiles(targetDirectory, "*.png");

            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);

                Mat actionImageMat = Cv2.ImRead(file, ImreadModes.Unchanged);

                if (actionImageMat.Empty())
                {
                    actionImageMat.Dispose();
                    continue;
                }

                bool matchFound = false;
                string cleanedFileName = fileName.Replace("action_", "").ToLower();

                foreach (Actions action in Enum.GetValues<Actions>())
                {
                    if (cleanedFileName == action.ToString().ToLower())
                    {
                        ActionTemplates[action] = actionImageMat;
                        matchFound = true;
                        break;
                    }
                }

                if (!matchFound)
                {
                    actionImageMat.Dispose();
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
        private static Mat ToBgr(Mat input)
        {
            if (input.Channels() == 4)
            {
                Mat output = new Mat();
                Cv2.CvtColor(input, output, ColorConversionCodes.BGRA2BGR);
                return output;
            }

            return input.Clone();
        }
    }
}
