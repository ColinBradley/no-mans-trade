using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using NoMansTrade.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NoMansTrade.Core
{
    public class AzureOcr : IDisposable
    {
        private readonly ComputerVisionClient mClient;

        public AzureOcr(string apiKey, string endPoint)
        {
            mClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(apiKey)) { Endpoint = endPoint };
        }

        public async Task<string> ExtractTextAsync(string imagePath)
        {
            using var imageStream = File.OpenRead(imagePath);

            return await this.ExtractTextAsync(imageStream);
        }

        public async Task<string> ExtractTextAsync(Stream imageStream)
        {
            var requestResult = await mClient.RecognizeTextInStreamAsync(imageStream, TextRecognitionMode.Printed);

            // Poll for result
            return await GetTextAsync(mClient, requestResult.OperationLocation);
        }

        public static (bool isBuying, Item[] items) ParseItems(string text)
        {
            var currentItem = new Item() { LastUpdate = DateTime.UtcNow };
            var items = new List<Item>() { currentItem };

            var isBuying = false;

            foreach (var line in text.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                switch (GuessType(line))
                {
                    case LineType.Name:

                        if (currentItem.Name.Length != 0)
                        {
                            currentItem = new Item() { LastUpdate = DateTime.UtcNow };
                            items.Add(currentItem);
                        }

                        currentItem.Name = line;

                        break;
                    case LineType.BuyFor:

                        isBuying = true;
                        currentItem.Price = GetEndNumbers(line);

                        break;
                    case LineType.SellFor:

                        isBuying = false;
                        currentItem.Price = GetEndNumbers(line);

                        break;
                    case LineType.PriceDifference:

                        double.TryParse(line.Trim().Trim('%'), out var difference);
                        currentItem.PriceDifferencePercentage = difference;

                        break;

                    case LineType.StockCount:

                        currentItem.Quantity = GetEndNumbers(line);

                        break;
                }
            }

            return (isBuying, items.Where(i => i.Name.Length > 0).ToArray());
        }

        private static int GetEndNumbers(string line)
        {
            var numberStr =
               new string(
                   line.Reverse()
                       .TakeWhile(c => c == ' ' || char.IsNumber(c) || c == ',' || c == '.')
                       .Where(char.IsNumber)
                       .Reverse()
                       .ToArray());

            if (int.TryParse(numberStr, out var number))
            {
                return number;
            }

            return -1;
        }

        private static LineType GuessType(string line)
        {
            if (line.Contains("Sell for", StringComparison.OrdinalIgnoreCase))
            {
                return LineType.SellFor;
            }

            if (line.Contains("Buy for", StringComparison.OrdinalIgnoreCase))
            {
                return LineType.BuyFor;
            }

            if (line.Contains("%", StringComparison.OrdinalIgnoreCase))
            {
                return LineType.PriceDifference;
            }

            if (line.Contains("/", StringComparison.OrdinalIgnoreCase))
            {
                return LineType.StockCount;
            }

            return LineType.Name;
        }

        private static async Task<string> GetTextAsync(ComputerVisionClient computerVision, string operationLocation)
        {
            var operationId = operationLocation.Substring(operationLocation.LastIndexOf('/') + 1);
            while (true)
            {
                var result = await computerVision.GetTextOperationResultAsync(operationId);
                Console.WriteLine("Polling for result...");
                if (result.Status == TextOperationStatusCodes.Succeeded)
                {
                    return string.Join("\n", result.RecognitionResult.Lines.Select(l => l.Text));
                }

                await Task.Delay(500);
            }
        }

        public void Dispose()
        {
            mClient.Dispose();
        }

        private enum LineType
        {
            SellFor,
            Name,
            PriceDifference,
            BuyFor,
            StockCount
        }
    }
}
