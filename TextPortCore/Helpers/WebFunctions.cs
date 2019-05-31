using System;
using System.IO;
using System.Net;
using System.Linq;

namespace TextPortCore.Helpers
{
    public static class WebFunctions
    {

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
