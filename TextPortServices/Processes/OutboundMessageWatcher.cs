using System;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Threading;
using Microsoft.EntityFrameworkCore;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPortServices.Processes
{
    public class OutboundMessageWatcher
    {
        private readonly TextPortContext _context;

        public OutboundMessageWatcher()
        {
            // Instantiate the DB context.
            //var optionsBuilder = new DbContextOptionsBuilder<TextPortContext>();
            //optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["TextPortContext"].ConnectionString);
            //this._context = new TextPortContext(optionsBuilder.Options);
        }

        public void Watch()
        {
            try
            {
                int loopCounter = 0;
                int sleepIntervalMilliseconds = getSleepInterval();
                int loopCountLimit = getLoopCountLimit();
                string messagePath = ConfigurationManager.AppSettings["SemaphoreFilesPath"];
                string bulkMessagePath = ConfigurationManager.AppSettings["BulkSemaphoreFilesPath"];
                int maxBulkMessagesToProcessPerLoop =  int.Parse(ConfigurationManager.AppSettings["MaxBulkMessagesToProcessPerLoop"]);

                while (loopCounter <= loopCountLimit)
                {
                    // Check for regular messages
                    DirectoryInfo baseFolder = new DirectoryInfo(messagePath);
                    FileInfo[] semaphoreFiles = baseFolder.GetFiles("*.sem");
                    foreach (FileInfo semaphoreFile in semaphoreFiles)
                    {
                        processSemaphoreFile(semaphoreFile);
                    }

                    // Check for bulk messages
                    baseFolder = new DirectoryInfo(bulkMessagePath);
                    semaphoreFiles = baseFolder.GetFiles("*.sem");
                    foreach (FileInfo semaphoreFile in semaphoreFiles.Take(maxBulkMessagesToProcessPerLoop))
                    {
                        processSemaphoreFile(semaphoreFile);
                    }

                    Thread.Sleep(sleepIntervalMilliseconds);
                    loopCounter++;
                }
            }
            catch (Exception ex)
            {
                EventLogging.WriteEventLogEntry("An error occurred in OutboundMessageWatcher.Watch. Message: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                Thread.CurrentThread.Abort();
            }
        }

        //private static void processSemaphoreFile(object source, FileSystemEventArgs e)
        private void processSemaphoreFile(FileInfo semaphoreFile)
        {
            try
            {
                int messageId = getMessageIDFromFile(semaphoreFile.FullName, semaphoreFile.Name);

                if (messageId > 0)
                {
                    using (TextPortDA da = new TextPortDA())
                    {
                        Message message = da.GetMessageById(messageId);

                        if (message.MessageId > 0)
                        {
                            Communications comms = new Communications();
                            if (comms.GenerateAndSendMessage(message))
                            {
                                message.ProcessingMessage += " Comms OK. ";
                                message.QueueStatus = 1;
                            }
                            else
                            {
                                message.ProcessingMessage += " Comms Failed. GenerateAndSendMessage failed. ";
                                message.QueueStatus = 2;
                            }
                        }
                        else
                        {
                            message.ProcessingMessage += " Comms Failed. Message ID not found.";
                            message.QueueStatus = 3;
                        }
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogging.WriteEventLogEntry("An error occurred in processSemaphoreFile. Message: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            try
            {
                File.Delete(semaphoreFile.FullName);
            }
            catch (Exception exDel)
            {
                EventLogging.WriteEventLogEntry("An error deleting semaphore file " + semaphoreFile.FullName + ".  Message: " + exDel.Message, System.Diagnostics.EventLogEntryType.Error);
            }
        }

        private int getMessageIDFromFile(string fullPathName, string fileName)
        {
            char[] delimiter = new char[] { '-' };
            int messageID = 0;

            try
            {
                if (File.Exists(fullPathName))
                {
                    string[] fileNameParts = fileName.Split(delimiter);

                    if (fileNameParts.Length == 2)
                    {
                        if (!String.IsNullOrEmpty(fileNameParts[0]) && !String.IsNullOrEmpty(fileNameParts[1]))
                        {
                            if (fileNameParts[0] == "semaphore")
                            {
                                string numericPart = fileNameParts[1].Substring(0, fileNameParts[1].IndexOf("."));
                                int.TryParse(numericPart, out messageID);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogging.WriteEventLogEntry("An error occurred in processSemaphoreFile. Message: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return messageID;
        }

        public int getSleepInterval()
        {
            int sleepIntervalSeconds = 2;

            try
            {
                int.TryParse(ConfigurationManager.AppSettings["CheckForSemaphoresEveryXSeconds"], out sleepIntervalSeconds);
            }
            catch (Exception)
            {
                sleepIntervalSeconds = 2;
            }

            return sleepIntervalSeconds * 1000; // Convert to milliseconds
        }

        public int getLoopCountLimit()
        {
            int loopCount = 100;

            try
            {
                int.TryParse(ConfigurationManager.AppSettings["SemaphoreLoopCountBeforeRestart"], out loopCount);
            }
            catch (Exception)
            {
                loopCount = 100;
            }

            return loopCount;
        }
    }
}