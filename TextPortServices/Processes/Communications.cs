using System;
using System.Threading;
using System.Configuration;

using TextPortCore.Models;
using TextPortCore.Helpers;
using TextPortCore.Integrations.Bandwidth;
using TextPortCore.Integrations.Nexmo;

namespace TextPortServices.Processes
{
    public class Communications
    {
        private int emailInterMessageWaitMs = 250;
        private int bandwidthInterMessageWaitMs = 1000;

        public bool GenerateAndSendMessage(Message message)
        {
            bool returnValue = false;

            getInterMessageWaitTimes();

            switch (message.DedicatedVirtualNumber.CarrierId)
            {
                case (int)Carriers.BandWidth:
                    using (Bandwidth bw = new Bandwidth())
                    {
                        returnValue = bw.RouteMessageViaBandwidthDotComGateway(message);
                    }
                    Thread.Sleep(bandwidthInterMessageWaitMs);
                    break;

                case (int)Carriers.Nexmo:
                    using (Nexmo nexmo = new Nexmo())
                    {
                        returnValue = nexmo.RouteMessageViaNexmoGateway(message);
                        Thread.Sleep(bandwidthInterMessageWaitMs);
                        break;
                    }
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
                int.TryParse(ConfigurationManager.AppSettings["NexmoInterMessageWaitMs"], out bandwidthInterMessageWaitMs);
            }
            catch (Exception)
            {
                emailInterMessageWaitMs = 250;
                bandwidthInterMessageWaitMs = 1000;
            }
        }
    }
}
