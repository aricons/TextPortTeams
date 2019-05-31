﻿using System;
using Microsoft.AspNet.SignalR;

namespace TextPort.Hubs
{
    public class HubFunctions : IDisposable
    {
        private IHubContext _hubContext;

        public HubFunctions()
        {
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<InboundHub>();
        }

        public void SendInboundMessageNotification(string userName, string fromNumber, string toNumber, string messageText, string messageHtml)
        {
            _hubContext.Clients.User(userName).messageNotification(fromNumber, toNumber, messageText, messageHtml);
        }

        public void SendDeliveryReceipt(string userName, string messageId, string messageHtml)
        {
            _hubContext.Clients.User(userName).deliveryReceipt(messageId, messageHtml);
        }

        public void SendBalanceUpdate(string userName, string balanceText)
        {
            _hubContext.Clients.User(userName).balanceUpdate(balanceText);
        }

        #region "Disposal"

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }
            // free native resources if there are any.
        }

        #endregion
    }
}