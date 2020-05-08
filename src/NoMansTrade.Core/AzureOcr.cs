using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using NoMansTrade.Core.Model;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NoMansTrade.Core
{
    public sealed class AzureOcr : IDisposable
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

        private static Regex sMatchQuantity = new Regex(@"\b^\d+\s*[|\\\/]\s*(\d+)\s*\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static (bool isBuying, Item[] items) ParseItems(string text)
        {
            var currentItem = new Item() { LastUpdate = DateTime.UtcNow };
            var items = new List<Item>() { currentItem };

            var isBuying = false;
            var index = 0;

            while (index < text.Length)
            {
                var current = text.Substring(index);

                if (current.StartsWith("Sell ", StringComparison.OrdinalIgnoreCase) || current.StartsWith("Buying ", StringComparison.OrdinalIgnoreCase))
                {
                    index += current.IndexOf("For", StringComparison.OrdinalIgnoreCase) + 3;

                    var (price, delta) = TakeNumbers(text.Substring(index));
                    if (delta <= 0)
                    {
                        index += 8;
                        continue;
                    }

                    isBuying = false;
                    currentItem.Price = price;
                    index += delta;
                    continue;
                }

                if (current.StartsWith("Buy ", StringComparison.OrdinalIgnoreCase))
                {
                    index += current.IndexOf("For", StringComparison.OrdinalIgnoreCase) + 3;

                    var (price, delta) = TakeNumbers(text.Substring(index));
                    if (delta <= 0)
                    {
                        index += 8;
                        continue;
                    }

                    isBuying = true;
                    currentItem.Price = price;
                    index += delta;
                    continue;
                }

                var quantityMatch = sMatchQuantity.Match(current);
                if (quantityMatch.Success)
                {
                    index += quantityMatch.Length;

                    if (!int.TryParse(quantityMatch.Groups[1].Value, out var quantity))
                    {
                        continue;
                    }

                    currentItem.Quantity = quantity;

                    continue;
                }

                // Probably dealing with name or some noise
                var endOfLine = current.IndexOf('\n', StringComparison.OrdinalIgnoreCase);
                if (endOfLine < 0)
                {
                    endOfLine = current.Length - 1;
                }

                var line = current.Substring(0, endOfLine + 1);
                index += line.Length;

                if (IsNoise(line))
                {
                    continue;
                }

                if (currentItem.Name.Length > 0)
                {
                    currentItem = new Item() { LastUpdate = DateTime.UtcNow };
                    items.Add(currentItem);
                }

                // -1 to remove newline at end
                currentItem.Name = line.Substring(0, line.Length -1);
            }

            if (!isBuying)
            {
                // Sale price is based on quantity, fix it!
                foreach (var item in items.ToArray())
                {
                    if (item.Price == 0)
                    {
                        // glitch :/
                        items.Remove(item);
                    }
                    if (item.Quantity == 0)
                    {
                        // Was selling 1... hopefully
                        continue;
                    }

                    item.Price = item.Price / item.Quantity;
                }
            }

            return (isBuying, items.Where(i => i.Name.Length > 0).ToArray());
        }

        private static readonly ParallelQuery<string> sNoiseStarts = new string[] {
            "Demand:",
            "Produced Locally",
            "Economy Demands",
            "Price:",
        }.AsParallel();

        private static bool IsNoise(string line)
        {
            return sNoiseStarts.Any(n => line.StartsWith(n, StringComparison.OrdinalIgnoreCase));
        }

        private static (int value, int end) TakeNumbers(string text)
        {
            var seenNumbers = false;
            var numbers = "";

            var index = 0;
            for (; index < text.Length; index++)
            {
                var character = text[index];
                if (char.IsNumber(character))
                {
                    seenNumbers = true;
                    numbers += character;
                    continue;
                }

                if (character == ',' || character == '.' || character == ' ')
                {
                    continue;
                }

                if (character == '\n')
                {
                    if (seenNumbers)
                    {
                        break;
                    }
                }
            }

            if (numbers.Length > 0 && int.TryParse(numbers, out var value))
            {
                return (value, index + 1);
            }

            return (-1, -1);
        }

        private static LineType GuessType(string text)
        {
            if (text.StartsWith("Sell for", StringComparison.OrdinalIgnoreCase))
            {
                return LineType.SellFor;
            }

            if (text.StartsWith("Buy for", StringComparison.OrdinalIgnoreCase))
            {
                return LineType.BuyFor;
            }

            if (text.Contains("/", StringComparison.OrdinalIgnoreCase))
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
            BuyFor,
            StockCount
        }
    }
}
