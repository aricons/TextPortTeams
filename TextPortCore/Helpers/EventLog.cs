using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TextPortCore.Helpers
{
    public static class EventLogging
    {
        public static bool WriteEventLogEntry(string eventMessage, EventLogEntryType eventType)
        {
            try
            {
                System.Diagnostics.EventLog.WriteEntry("TextPort V2 Services", eventMessage, eventType);

                return true;
            }
            catch (Exception ex)
            {
                //ErrorHandling eh = new ErrorHandling(_context);
                //eh.LogException("Helpers.EventLog.WriteEventLogEntry", ex);
                return false;
            }
        }
    }
}
