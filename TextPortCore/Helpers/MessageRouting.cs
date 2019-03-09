using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TextPortCore.Models;

namespace TextPortCore.Helpers
{
    public static class MessageRouting
    {
        public static bool WriteSemaphoreFile(Message message)
        {
            try
            {
                string fileName = $"semaphore-{message.MessageId}.sem";
                string semaphoreFilesPath = ConfigurationManager.AppSettings["SemaphoreFilesPath"];

                if (Directory.Exists(semaphoreFilesPath))
                {
                    StreamWriter writer = new StreamWriter($"{semaphoreFilesPath}{fileName}");
                    writer.Write(message.MessageId.ToString());
                    writer.Close();
                    writer = null;
                    message.ProcessingMessage += "Semaphore written OK. ";
                    return true;
                }
                else
                {
                    message.ProcessingMessage += $"Could not write semaphore file to folder {semaphoreFilesPath}.";
                }
            }
            catch (Exception ex)
            {
                message.ProcessingMessage += $"Exception in writeQueueSemaphoreFile: {ex.Message}.";
            }
            return false;
        }
    }
}
