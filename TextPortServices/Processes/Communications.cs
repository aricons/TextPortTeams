using System;
using System.Threading;
using System.Configuration;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Integrations.Bandwidth;

namespace TextPortServices.Processes
{
    public class Communications
    {
        private readonly TextPortContext _context;
        private int emailInterMessageWaitMs = 250;
        private int nexmoInterMessageWaitMs = 1000;

        public Communications(TextPortContext context)
        {
            this._context = context;
        }

        public bool GenerateAndSendMessage(Message message)
        {
            bool returnValue = false;

            getInterMessageWaitTimes();

            switch (message.RoutingType)
            {
                //case "Nexmo":
                //    returnValue = Nexmo.RouteMessageViaNexmoGateway(ref message, "INTERNATIONAL");
                //    Thread.Sleep(nexmoInterMessageWaitMs);
                //    break;

                case "Bandwidth":
                    using (Bandwidth bw = new Bandwidth(_context))
                    {
                        returnValue = bw.RouteMessageViaBandwidthDotComGateway(message);
                        _context.SaveChanges();
                    }
                    Thread.Sleep(nexmoInterMessageWaitMs);
                    break;

                //case "InfoBip":
                //    returnValue = InfoBip.InfoBip.RouteMessageViaInfoBipGateway(ref message);
                //    Thread.Sleep(nexmoInterMessageWaitMs);
                //    break;

                default:
                    returnValue = false;
                    break;
            }
            return returnValue;
        }

        private void getInterMessageWaitTimes()
        {
            try
            {
                int.TryParse(ConfigurationManager.AppSettings["EmailInterMessageWaitMs"], out emailInterMessageWaitMs);
                int.TryParse(ConfigurationManager.AppSettings["NexmoInterMessageWaitMs"], out nexmoInterMessageWaitMs);
            }
            catch (Exception)
            {
                emailInterMessageWaitMs = 250;
                nexmoInterMessageWaitMs = 1000;
            }
        }
    }
}
