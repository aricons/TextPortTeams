using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Data.OleDb;
using System.Text.RegularExpressions;

namespace TextPortCore.Helpers
{
    public static class UploadUtilities
    {
        static Regex csvSplit = new Regex("(?:^|,)(\"(?:[^\"]+|\"\")*\"|[^,]*)", RegexOptions.Compiled);

        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                dt.Columns.Add("Number");
                dt.Columns.Add("Message");

                while (!sr.EndOfStream)
                {
                    string[] row = splitCSV(sr.ReadLine());
                    if (row.Length > 1)
                    {
                        DataRow dr = dt.NewRow();
                        // Add the number
                        dr["Number"] = row[0].Trim().Trim('"');

                        // Add the message
                        string message = string.Empty;
                        for (int i = 1; i < row.Length; i++)
                        {
                            message += row[i];
                        }
                        if (!string.IsNullOrEmpty(message))
                        {
                            dr["Message"] = message.Trim('"');
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            return dt;
        }

        public static DataTable ConvertXSLXtoDataTable(string strFilePath, string connString)
        {
            OleDbConnection oledbConn = new OleDbConnection(connString);
            DataTable dt = new DataTable();
            try
            {
                oledbConn.Open();
                DataTable dbSchema = oledbConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dbSchema == null || dbSchema.Rows.Count < 1)
                {
                    throw new Exception("Error: Could not determine the name of the first worksheet.");
                }
                string firstSheetName = dbSchema.Rows[0]["TABLE_NAME"].ToString();

                using (OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + firstSheetName + "]", oledbConn))
                {
                    OleDbDataAdapter oleda = new OleDbDataAdapter();
                    oleda.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    oleda.Fill(ds);

                    dt = ds.Tables[0];
                }
            }
            catch
            {
            }
            finally
            {
                oledbConn.Close();
            }
            return dt;
        }

        private static string[] splitCSV(string input)
        {
            List<string> list = new List<string>();
            string curr = null;
            foreach (Match match in csvSplit.Matches(input))
            {
                curr = match.Value;
                if (0 == curr.Length)
                {
                    list.Add("");
                }

                list.Add(curr.TrimStart(','));
            }

            return list.ToArray();
        }

    }
}
