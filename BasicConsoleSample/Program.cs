// 
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
// 
// Microsoft Cognitive Services: http://www.microsoft.com/cognitive
// 
// Microsoft Cognitive Services Github:
// https://github.com/Microsoft/Cognitive
// 
// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

using System;
using VideoFrameAnalyzer;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System.Net.Http;
using System.Web;
using System.Net.Http.Headers;
using System.Text;

namespace BasicConsoleSample
{
    internal class Program
    {
        static async void MakeRequest()
        {
            try {
                var client = new HttpClient();
                var queryString = HttpUtility.ParseQueryString(string.Empty);

                // Request headers
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "f6d3edb3ecf14397a5989e4533f4aba4");

                // Request parameters
                queryString["returnFaceId"] = "true";
                queryString["returnFaceLandmarks"] = "false";
                queryString["returnFaceAttributes"] = "Age";
                var uri = "https://westus.api.cognitive.microsoft.com/face/v1.0/detect?" + queryString;

                HttpResponseMessage response;

                // Request body
                byte[] byteData = Encoding.UTF8.GetBytes("{ \"url\":\"https://lh5.googleusercontent.com/-AI__M0nZDU4/AAAAAAAAAAI/AAAAAAAAAGs/P5tdI3rFaFs/s0-c-k-no-ns/photo.jpg \"}");


                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    response = await client.PostAsync(uri, content);

                    Console.Write(response.StatusCode);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
        private static void Main(string[] args)
        {


            MakeRequest();
            Console.ReadLine();
            Console.WriteLine("Ok fine");
            return;
            // Create grabber. 
            FrameGrabber<Face[]> grabber = new FrameGrabber<Face[]>();

            // Create Face API Client.
            FaceServiceClient faceClient = new FaceServiceClient("<subscription key>");

            // Set up a listener for when we acquire a new frame.
            grabber.NewFrameProvided += (s, e) =>
            {
                Console.WriteLine("New frame acquired at {0}", e.Frame.Metadata.Timestamp);
            };

            // Set up Face API call.
            grabber.AnalysisFunction = async frame =>
            {
                Console.WriteLine("Submitting frame acquired at {0}", frame.Metadata.Timestamp);
                // Encode image and submit to Face API. 
                return await faceClient.DetectAsync(frame.Image.ToMemoryStream(".jpg"));
            };

            // Set up a listener for when we receive a new result from an API call. 
            grabber.NewResultAvailable += (s, e) =>
            {
                if (e.TimedOut)
                    Console.WriteLine("API call timed out.");
                else if (e.Exception != null)
                    Console.WriteLine("API call threw an exception.");
                else
                    Console.WriteLine("New result received for frame acquired at {0}. {1} faces detected", e.Frame.Metadata.Timestamp, e.Analysis.Length);
            };

            // Tell grabber when to call API.
            // See also TriggerAnalysisOnPredicate
            grabber.TriggerAnalysisOnInterval(TimeSpan.FromMilliseconds(3000));

            // Start running in the background.
            grabber.StartProcessingCameraAsync().Wait();

            // Wait for keypress to stop
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();

            // Stop, blocking until done.
            grabber.StopProcessingAsync().Wait();
        }
    }
}
