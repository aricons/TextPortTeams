using System;
using System.Text;
using System.Net;
using System.IO;

namespace TextPortCore.Integrations
{
    public static class REST
    {
        const string apiToken = "t-h5xlmcsiydxo5ye7l4q7why";
        const string apiSecret = "uw7ybc3i5vqatixqktw5wtrlz5gk3jnds5dbchq";

        public static string GetData(string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Accept = "application/json";
            request.Method = "GET";

            string _auth = String.Format("{0}:{1}", apiToken, apiSecret);
            string _enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(_auth));
            string _cred = string.Format("{0} {1}", "Basic", _enc);
            request.Headers[HttpRequestHeader.Authorization] = _cred;

            // Make the request and get the response
            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                string foo = ex.Message;
                return String.Empty;
            }
        }

        public static string PostData(string url, string data)
        {
            HttpWebRequest request = null;

            Uri uri = new Uri(url + data);
            request = (HttpWebRequest)WebRequest.Create(uri);

            request.Method = "POST";
            request.ContentType = "text/xml;charset=utf-8";
            request.ContentLength = data.Length;

            using (Stream writeStream = request.GetRequestStream())
            {
                byte[] bytes = Encoding.ASCII.GetBytes(data);
                writeStream.Write(bytes, 0, bytes.Length);
                writeStream.Close();
            }

            string result = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        result = readStream.ReadToEnd();
                    }
                }
            }
            return result;
        }

        public static string Delete(string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Accept = "application/xml";
            request.Method = "DELETE";

            // Make the request and get the response
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                return reader.ReadToEnd();
            }
        }
    }
}