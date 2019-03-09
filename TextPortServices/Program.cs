using System;
using System.Reflection;
using System.Configuration.Install;
using System.ServiceProcess;
using TextPortCore.Helpers;

namespace TextPortServices
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {

            if (!Environment.UserInteractive)
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new TextPortServices()
                };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                //string parameter = string.Concat(args);
                //switch (parameter)
                //{
                //    case "--install":
                //        ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
                //        break;
                //    case "--uninstall":
                //        ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
                //        break;
                //}

                // Manually create service
                TextPortServices services = new TextPortServices();
                EventLogging.WriteEventLogEntry("The TextPort Communications Service is starting manually.", System.Diagnostics.EventLogEntryType.Information);
                services.ManualStart();
            }
        }
    }
}
