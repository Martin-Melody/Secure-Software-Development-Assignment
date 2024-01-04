using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SSD_Assignment___Banking_Application.Services
{
    public class EventLogService
    {

        public void WriteToEventLog(string message, EventLogEntryType type, int eventId)
        {
            string source = "SecureSoftwareDevelopmentProject"; 

            // Create the source if it does not exist.
            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, "Application"); 
            }

            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = source;
                eventLog.WriteEntry(message, type, eventId);
            }
        }

    }
}
