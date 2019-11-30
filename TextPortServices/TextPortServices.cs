using System;
using System.ServiceProcess;
using System.Threading;
using TextPortCore.Helpers;
using TextPortServices.Processes;

namespace TextPortServices
{
    public partial class TextPortServices : ServiceBase
    {
        //public TextPortServices()
        //{
        //    InitializeComponent();
        //}

        //protected override void OnStart(string[] args)
        //{
        //}

        //protected override void OnStop()
        //{
        //}

        private bool stopService = false;
        private DateTime startTime;
        private DateTime lastUpdateTime;

        Thread outboundMessageThread = null;
        Thread watcherThread = null;
        Thread virtualNumberExpirationsThread = null;
        //Thread emailNotificationsThread = null;
        //Thread virtualNumbersAuditThread = null;

        public TextPortServices()
        {
            InitializeComponent();

            this.startTime = DateTime.Now;
            this.lastUpdateTime = this.startTime;
        }

        private void InitializeComponent()
        {
            this.ServiceName = "TextPort Communications Service";
        }

        // Start the Windows service.
        protected override void OnStart(string[] args)
        {
            this.stopService = false;

            // Start the watcher thread
            watcherThread = new Thread(new ThreadStart(startOutboundMessageWatcher));
            watcherThread.Start();

            // Start the virtual number expirations poller.
            virtualNumberExpirationsThread = new Thread(new ThreadStart(startVirtualNumberExpirationsPolling));
            virtualNumberExpirationsThread.Start();

            // Start the incoming message email notifications poller.
            //emailNotificationsThread = new Thread(new ThreadStart(startEmailNotificationsPolling));
            //emailNotificationsThread.Start();

            // Start the virtual numbers audit thread
            //virtualNumbersAuditThread = new Thread(new ThreadStart(startVirtualNumbersAuditPolling));
            //virtualNumbersAuditThread.Start();

            EventLogging.WriteEventLogEntry("TextPort Communications Service started.", System.Diagnostics.EventLogEntryType.Information);
        }

        protected override void OnStop()
        {
            this.stopService = true;

            watcherThread.Abort();

            EventLogging.WriteEventLogEntry("TextPort Communications Service stopped.", System.Diagnostics.EventLogEntryType.Information);
        }

        public void ManualStart()
        {
            this.OnStart(null);
        }

        public void startOutboundMessageWatcher()
        {
            int threadCheckIntervalSeconds = 60;

            while (!stopService)
            {
                try
                {
                    if (DateTime.Now.Subtract(lastUpdateTime).TotalHours >= 2)
                    {
                        TimeSpan elapsed = DateTime.Now.Subtract(startTime);
                        string elapsedTime = string.Format("The TextPort communications agent has been running for: {0:0} days, {1}:{2:00} hours", Math.Floor(elapsed.TotalDays), elapsed.Hours, elapsed.Minutes);
                        lastUpdateTime = DateTime.Now;
                        EventLogging.WriteEventLogEntry(elapsedTime, System.Diagnostics.EventLogEntryType.Information);
                    }

                    if (outboundMessageThread == null)
                    {
                        OutboundMessageWatcher watcher = new OutboundMessageWatcher();
                        outboundMessageThread = new Thread(watcher.Watch);
                        outboundMessageThread.Name = "Outbound message watcher thread";
                        outboundMessageThread.Start();
                    }

                    if (outboundMessageThread.ThreadState == ThreadState.Stopped)
                    {
                        outboundMessageThread.Abort();
                        outboundMessageThread = null;
                        GC.Collect();
                        EventLogging.WriteEventLogEntry("TextPort communications service was detected as stopped. Restarting.", System.Diagnostics.EventLogEntryType.Warning);
                    }
                    else
                    {
                        sleep(threadCheckIntervalSeconds, false);
                    }
                }

                catch (Exception ex)
                {
                    EventLogging.WriteEventLogEntry("An exception occurred in the TextPort.com communications service. Message: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                    sleep(threadCheckIntervalSeconds, false);
                }
            }
        }

        public void startVirtualNumberExpirationsPolling()
        {
            int sleepIntervalSeconds = VnExpirationsPoller.GetPollingInterval();

            while (!stopService)
            {
                try
                {
                    VnExpirationsPoller.CheckForVirtualNumberExpirations();
                    //sleep(60, false); for testing
                    sleep(sleepIntervalSeconds, false);
                }

                catch (Exception ex)
                {
                    EventLogging.WriteEventLogEntry("An exception occurred in the TextPort.com communications service in startVirtualNumberExpirationsPolling. Message: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                    sleep(10, false);
                }
            }
        }

        //public void startEmailNotificationsPolling()
        //{
        //    int sleepIntervalSeconds = EmailNotifications.GetPollingInterval();

        //    while (!stopService)
        //    {
        //        try
        //        {
        //            EmailNotifications.CheckForNewNotifications();
        //            sleep(sleepIntervalSeconds, false);
        //        }

        //        catch (Exception ex)
        //        {
        //            EventLogging.WriteEventLogEntry("An exception occurred in the TextPort.com communications service in startEmailNotificationsPolling. Message: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
        //            sleep(10, false);
        //        }
        //    }
        //}

        //public void startVirtualNumbersAuditPolling()
        //{
        //    int sleepIntervalSeconds = VirtualNumberAudit.GetPollingInterval();

        //    while (!stopService)
        //    {
        //        try
        //        {
        //            VirtualNumberAudit.RunAudit();
        //            sleep(sleepIntervalSeconds, false);
        //        }

        //        catch (Exception ex)
        //        {
        //            EventLogging.WriteEventLogEntry("An exception occurred in the TextPort.com communications service in startVirtualNumbersAuditPolling. Message: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
        //            sleep(10, false);
        //        }
        //    }
        //}

        private bool isOutboundMessageThreadRunning()
        {
            if (stopService)
                return false;

            if (outboundMessageThread == null)
                return false;

            if (outboundMessageThread.ThreadState != ThreadState.Stopped)
                return true;

            // The thread is not running
            return false;
        }

        private void sleep(int seconds, bool ignoreStop)
        {
            DateTime now = DateTime.Now;

            while (DateTime.Now.Subtract(now).TotalSeconds < seconds)
            {
                if ((!ignoreStop) && (stopService)) return;
                Thread.Sleep(500);
            }
        }
    }
}
