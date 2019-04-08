using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ImageId
{
    public class Program
    {
        private static readonly string _subscriptionKey = "";
        private static readonly string _cognitiveVisionEndpoint = "https://westcentralus.api.cognitive.microsoft.com";

        private static readonly List<VisualFeatureTypes> _featuresToReturn = new List<VisualFeatureTypes>();

        public static void Main(string[] args)
        {
            SetupFeaturesToReturn();
            string imageToAnalyze = PromptForImageUrl();

            Console.WriteLine("Analyzing image....");

            using (var computerVisionClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(_subscriptionKey), new DelegatingHandler[] { }))
            {
                computerVisionClient.Endpoint = _cognitiveVisionEndpoint;

                AnalyzeImageByUrlAsync(computerVisionClient, imageToAnalyze).Wait();
            }
            Console.WriteLine("Q to quit, or Enter to try again.");
            switch (Console.ReadLine().ToUpper()){
                case "Q":
                    return;
                default:
                    Main( new string[0]);
                    break;
            }
            return;
        }

        private static async Task AnalyzeImageByUrlAsync(ComputerVisionClient computerVisionClient, string imageUrl)
        {
            ImageAnalysis imageAnalysis = await computerVisionClient.AnalyzeImageAsync(imageUrl, _featuresToReturn);
            DisplayResults(imageAnalysis, imageUrl);
        }

        private static void SetupFeaturesToReturn()
        {
            _featuresToReturn.Add(VisualFeatureTypes.Categories);
            _featuresToReturn.Add(VisualFeatureTypes.Description);
            _featuresToReturn.Add(VisualFeatureTypes.Objects);
            _featuresToReturn.Add(VisualFeatureTypes.Tags);
        }

        private static void DisplayResults(ImageAnalysis imageAnalysis, string imageUrl)
        {
            Console.WriteLine($"This image looks like...{imageAnalysis.Description.Captions[0].Text}");
            Console.WriteLine("Here are some tags to describe it: ");
            Console.WriteLine(string.Join(",", imageAnalysis.Description.Tags));
        }

        private static string PromptForImageUrl()
        {
            Console.WriteLine("Provide a url to an image to analyze and press enter.");
            string imageUrl = Console.ReadLine();
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                Console.WriteLine("Invalid image url. Try again!");
                PromptForImageUrl();
            }
            return imageUrl;
        }
    }
}