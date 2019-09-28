using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;

namespace TextPortCore.Helpers
{
    public static class WebFunctions
    {
        public static string MakeHttpPost(string url, string content)
        {
            //Uri uri = new Uri(url);
            string responseString = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            var postData = "thing1=" + Uri.EscapeDataString("hello");
            postData += "&thing2=" + Uri.EscapeDataString("world");
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "text/json";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            //responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                responseString = sr.ReadToEnd();
            }

            return responseString;
        }

        public static string GetImageFromURL(string url, int accountId)
        {
            Uri uri = new Uri(url);
            
            string localFileName = $"{RandomString.RandomNumber()}_{uri.Segments.Last()}";

            using (WebClient client = new WebClient())
            {
                client.UseDefaultCredentials = true;
                client.Credentials = new NetworkCredential(Constants.Bandwidth.ApiToken, Constants.Bandwidth.ApiSecret);

                byte[] imageBytes = client.DownloadData(uri);

                if (imageBytes.Length > 0)
                {
                    var fileHandler = new FileHandling();
                    if (fileHandler.SaveMMSFile(new MemoryStream(imageBytes), accountId, localFileName, false))
                    {
                        return localFileName;
                    }
                }
            }

            return string.Empty;
        }
    }
}
