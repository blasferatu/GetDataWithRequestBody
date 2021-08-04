#region using

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

#endregion

namespace GetDataWithRequestBody
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var admUnits = new List<string>() { "BMA", "ESMC" };
                var admUnitsJson = new JavaScriptSerializer().Serialize(admUnits); // Serialize request body data without using external packages.

                var data = GetDataWithRequestBody("https://webservices.libware.net/LbwLibraryData/api/clientData/AGUEDA/users", admUnitsJson);
                Console.WriteLine(data);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: {e.Message}");
            }
            Console.WriteLine();
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        /// <summary>
        /// Gets JSON data from a Web API endpoint using a GET method with data in the request body in .NET Framework 4.x.
        /// This method is useful if HttpClient cannot be used (e.g. no NuGet packages can be used). In .NET 5 or above, HttpClient can be used instead.
        /// </summary>
        /// <param name="url">Web API endpoint URL.</param>
        /// <param name="jsonBodyData">JSON-serialised body data.</param>
        /// <returns>String with JSON data.</returns>
        private static string GetDataWithRequestBody(string url, string jsonBodyData)
        {
            // How to use HttpClient to send content in body of GET request?
            // https://stackoverflow.com/questions/43421126/how-to-use-httpclient-to-send-content-in-body-of-get-request

            var jsonData = "";

            var request = WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "GET";

            SetContentBodyNotAllowedToFalse(ref request);

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                streamWriter.Write(jsonBodyData);

            var response = (HttpWebResponse)request.GetResponse();

            var dataStream = response.GetResponseStream();

            using (StreamReader reader = new StreamReader(dataStream, Encoding.UTF8))
                jsonData = reader.ReadToEnd();

            dataStream.Close();

            return jsonData;
        }

        /// <summary>
        /// Sets a WebRequest's ContentBodyNotAllowed property to false in order to be possible to send body data
        /// with a GET verb in .NET Framework 4.x (this is not necessary in .NET 5. HttpClient can be used instead).
        /// </summary>
        /// <param name="request">WebRequest instance.</param>
        private static void SetContentBodyNotAllowedToFalse(ref WebRequest request)
        {
            var type = request.GetType();
            var currentMethod = type.GetProperty("CurrentMethod", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(request);

            var methodType = currentMethod.GetType();
            methodType.GetField("ContentBodyNotAllowed", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(currentMethod, false);
        }
    }
}
