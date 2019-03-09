using System;

using TextPortCore.Models;

namespace TextPortCore.Data
{
    public partial class TextPortDA
    {
        #region "Insert Methods"

        public bool InsertErrorLogItem(string programName, Exception ex, string details)
        {
            try
            {
                ErrorLogItem errorLogItem = new ErrorLogItem()
                {
                    Details = details,
                    ErrorDateTime = DateTime.UtcNow,
                    ErrorMessage = String.Format("Message: {0}. Stack Trace {1}", ex.Message, ex.StackTrace),
                    ProgramName = programName
                };

                _context.ErrorLog.Add(errorLogItem);

                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

    }
}