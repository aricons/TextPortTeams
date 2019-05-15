using System.Configuration;

namespace TextPortCore.Helpers
{
    public static class MMSUtils
    {
        public static string GetMMSFileURL(int accountId, int storageId, string fileName)
        {
            return $"{ConfigurationManager.AppSettings["MMSImagesBaseUrl"]}{accountId}/{fileName}";
        }
    }
}
