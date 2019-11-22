using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;

using TextPortCore.Models;

namespace TextPortCore.Helpers
{
    public class FileHandling
    {
        private readonly string mmsFilePath = ConfigurationManager.AppSettings["MMSFilePath"];
        private readonly string uploadsFilePath = ConfigurationManager.AppSettings["UploadFilesBasePath"];

        public bool SaveMMSFile(Stream strm, int accountId, string fileName, bool temporaryFile)
        {
            try
            {
                string basePath = this.mmsFilePath;

                if (temporaryFile)
                {
                    basePath = $"{mmsFilePath}Temp\\";
                    if (!Directory.Exists($"{basePath}"))
                    {
                        Directory.CreateDirectory(basePath);
                    }
                }

                basePath = $"{basePath}{accountId}\\";
                if (!Directory.Exists($"{basePath}"))
                {
                    Directory.CreateDirectory(basePath);
                }

                if (strm != null && !string.IsNullOrEmpty(basePath) && !string.IsNullOrEmpty(fileName))
                {
                    string saveFileName = $"{basePath}{fileName}";

                    using (var fileStream = File.Create(saveFileName))
                    {
                        strm.Seek(0, SeekOrigin.Begin);
                        strm.CopyTo(fileStream);

                        return (File.Exists(saveFileName)) ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("FileHandling.SaveMMSFile", ex);
            }
            return false;
        }

        public bool DeleteMMSFile(int accountId, string fileName, bool temporaryFile)
        {
            string basePath = this.mmsFilePath;

            if (temporaryFile)
            {
                basePath = $"{mmsFilePath}Temp\\";
            }

            basePath = $"{basePath}{accountId}\\";
            if (!string.IsNullOrEmpty(basePath) && !string.IsNullOrEmpty(fileName))
            {
                string deleteFileName = $"{basePath}{fileName}";
                try
                {
                    File.Delete(deleteFileName);
                    return (!File.Exists(deleteFileName)) ? true : false;
                }
                catch (Exception ex)
                {
                    ErrorHandling eh = new ErrorHandling();
                    eh.LogException("FileHandling.DeleteMMSFile", ex);
                }
            }
            return false;
        }

        public List<MMSFile> GetMMSFilesForMessage(int accountId, string mmsFileNames)
        {
            List<MMSFile> mmsFiles = new List<MMSFile>();

            if (!string.IsNullOrEmpty(mmsFileNames))
            {
                string[] fileNamesList = { mmsFileNames };

                if (mmsFileNames.Contains(","))
                {
                    fileNamesList = mmsFileNames.Split(',');
                }

                foreach (string mmsFileName in fileNamesList)
                {
                    string filePath = $"{mmsFilePath}{mmsFileName}";
                    if (File.Exists(filePath))
                    {
                        MMSFile mmsFile = new MMSFile();
                        mmsFile.FileName = filePath;
                        //mmsFile.DataBytes = File.ReadAllBytes(filePath);
                        mmsFiles.Add(mmsFile);
                    }
                }
            }

            return mmsFiles;
        }

        public bool SaveUploadFile(Stream strm, int accountId, string fileName, ref string fullPathName)
        {
            try
            {
                fullPathName = string.Empty;
                string basePath = this.uploadsFilePath;

                basePath = $"{basePath}{accountId}\\";
                if (!Directory.Exists($"{basePath}"))
                {
                    Directory.CreateDirectory(basePath);
                }

                if (strm != null && !string.IsNullOrEmpty(basePath) && !string.IsNullOrEmpty(fileName))
                {
                    string saveFileName = $"{basePath}{fileName}";

                    using (var fileStream = File.Create(saveFileName))
                    {
                        strm.Seek(0, SeekOrigin.Begin);
                        strm.CopyTo(fileStream);

                        fullPathName = saveFileName;
                        return (File.Exists(saveFileName)) ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("FileHandling.SaveUploadFile", ex);
            }
            return false;
        }
    }
}
