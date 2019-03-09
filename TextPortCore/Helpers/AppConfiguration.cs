//using Microsoft.Extensions.Configuration;
using System.IO;

namespace TextPortCore.AppConfig
{
    public class AppConfiguration
    {
        //public readonly string _connectionString = string.Empty;
        private string apiLogFiles = string.Empty;

        public string APILogFiles
        {
            get { return this.apiLogFiles; }
        }

        //public AppConfiguration()
        //{
        //    ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
        //    string path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
        //    //configurationBuilder.AddJsonFile(path, false);

        //    IConfigurationRoot root = configurationBuilder.Build();

        //    //_connectionString = root.GetSection("ConnectionString").GetSection("DataConnection").Value;

        //    //apiLogFiles = root.GetSection("").GetSection("").Value;

        //    var appSettings = root.GetSection("ApplicationSettings");

        //    apiLogFiles = appSettings.GetSection("APILogFiles").Value;
        //}
    }
}