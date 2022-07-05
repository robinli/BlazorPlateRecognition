using System;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Linq;

namespace ImageResizeOCRBlazorWasm.Server
{
    public class ImageRecognizer
    {
        // Add your Computer Vision subscription key and endpoint
        static string subscriptionKey = "YOUR_KEY_1";
        static string endpoint = "YOUR_Endpoint";

        public async Task<string> Process(Stream imageStream)
        {
            ComputerVisionClient client = Authenticate(endpoint, subscriptionKey);
            // Extract text (OCR) from a URL image using the Read API
            return await ReadFileStream(client, imageStream);
        }

        public ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            return client;
        }

        public async Task<string> ReadFileStream(ComputerVisionClient client, Stream imageStream)
        {
            string image_text = "";

            // Read text from URL
            var textHeaders = await client.ReadInStreamAsync(imageStream);
            // After the request, get the operation location (operation ID)
            string operationLocation = textHeaders.OperationLocation;

            // Retrieve the URI where the extracted text will be stored from the Operation-Location header.
            // We only need the ID and not the full URL
            const int numberOfCharsInOperationId = 36;
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            // Extract the text
            ReadOperationResult results;
            do
            {
                results = await client.GetReadResultAsync(Guid.Parse(operationId));
            }
            while ((results.Status == OperationStatusCodes.Running ||
                results.Status == OperationStatusCodes.NotStarted));

            // Display the found text.
            var textUrlFileResults = results.AnalyzeResult.ReadResults;
            int biggestArea = 0;
            foreach (ReadResult page in textUrlFileResults)
            {
                foreach (Line line in page.Lines)
                {
                    //以最大面積當作車牌號碼
                    int area = CalculateArea(line.BoundingBox);
                    if (area > biggestArea)
                    {
                        biggestArea = area;
                        image_text = line.Text;
                    }
                }
            }
            return image_text;
        }

        public record Point
        {
            public int X { get; set; }
            public int Y { get; set; }

        }
        private int CalculateArea(IList<double?> boundingBox)
        {
            if (boundingBox == null)
            {
                return 0;
            }

            List<Point> points = new List<Point>();
            for(int i = 0; i < boundingBox.Count; i+=2)
            {
                points.Add(new Point { X = Convert.ToInt32(boundingBox[i]), Y = Convert.ToInt32(boundingBox[i+1]) });
            }
            points.Add(points[0]);

            var area = Math.Abs(points.Take(points.Count - 1)
               .Select((p, i) => (points[i + 1].X - p.X) * (points[i + 1].Y + p.Y))
               .Sum() / 2);
            return area;
        }
    }
}