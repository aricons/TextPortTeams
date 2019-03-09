using System;
using System.Diagnostics;

using TextPortCore.Data;

namespace TextPortCore.Helpers
{
    public class ErrorHandling
    {
        private readonly TextPortContext _context;

        public ErrorHandling(TextPortContext context)
        {
            this._context = context;
        }

        public bool LogException(string programName, Exception ex)
        {
            try
            {
                if (ex != null)
                {
                    using (TextPortDA da = new TextPortDA(_context))
                    {
                        return da.InsertErrorLogItem(programName, ex, string.Empty);
                    }
                }
            }
            catch (Exception)
            {
                // This code should never be hit
            }

            return false;
        }

        public bool LogException(string programName, Exception ex, string details)
        {
            try
            {
                if (ex != null)
                {
                    using (TextPortDA da = new TextPortDA(_context))
                    {
                        return da.InsertErrorLogItem(programName, ex, details);
                    }
                }
            }
            catch (Exception)
            {
                // This code should never be hit
            }

            return false;
        }

        //public static bool WriteEventLogEntry(string eventMessage, Exception ex, EventLogEntryType eventType)
        //{
        //    try
        //    {
        //        string exceptionMessage = String.Format("Exception message: {0}{1}Stack Trace: {2}", ex.Message, Environment.NewLine, ex.StackTrace);
        //        EventLog.WriteEntry("TextPort System", String.Format("{0}{1}{2}", eventMessage, Environment.NewLine, exceptionMessage), eventType);

        //        return true;
        //    }
        //    catch (Exception ex1)
        //    {
        //        string foo = ex1.Message;
        //        return false;
        //    }
        //}

    }

}