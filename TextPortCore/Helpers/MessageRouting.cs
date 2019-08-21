using System;
using System.IO;
using System.Configuration;

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
                string semaphoreFilesPath = string.Empty;
                string processingMessage = string.Empty;

                switch (message.MessageType)
                {
                    case (int)MessageTypes.Bulk:
                    case (int)MessageTypes.BulkUpload:
                        semaphoreFilesPath = ConfigurationManager.AppSettings["BulkSemaphoreFilesPath"];
                        processingMessage = "Bulk semaphore file created. ";
                        break;
                    default:
                        semaphoreFilesPath = ConfigurationManager.AppSettings["SemaphoreFilesPath"];
                        processingMessage = "Regular semaphore file created. ";
                        break;
                };

                if (Directory.Exists(semaphoreFilesPath))
                {
                    StreamWriter writer = new StreamWriter($"{semaphoreFilesPath}{fileName}");
                    writer.Write(message.MessageId.ToString());
                    writer.Close();
                    writer = null;

                    message.ProcessingMessage += processingMessage;
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
