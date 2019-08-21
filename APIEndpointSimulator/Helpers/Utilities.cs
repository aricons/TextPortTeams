using System;
using System.IO;
using System.Configuration;

using TextPortCore.Helpers;

namespace APIEndpointSimulator.Helpers
{
    public static class Utilities
    {
        public static bool WriteLogTextToDisk(string logText)
        {
            File.WriteAllText(getLogFileName(), logText);
            return true;
        }

        private static string getLogFileName()
        {
            return ConfigurationManager.AppSettings["APISimulatorFilesFolder"] + $"{DateTime.Now:yyyy-mm-dd}-{RandomString.RandomNumber()}.txt";
        }
    }
}