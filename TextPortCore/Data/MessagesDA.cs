using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using TextPortCore.Models;
using TextPortCore.Helpers;


namespace TextPortCore.Data
{
    public partial class TextPortDA
    {
        #region "Select Methods"

        public Message GetMessageById(int messageId)
        {
            try
            {
                Message msg = _context.Messages.Include(m => m.MMSFiles).Where(x => x.MessageId == messageId).FirstOrDefault();
                // Get the virtual number.
                if (msg.VirtualNumberId > 0)
                {
                    DedicatedVirtualNumber dvn = _context.DedicatedVirtualNumbers.FirstOrDefault(x => x.VirtualNumberId == msg.VirtualNumberId);
                    if (dvn != null)
                    {
                        msg.VirtualNumber = dvn.VirtualNumber;
                    }
                }
                return msg;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("MessagesDA.GetMessageById", ex);
            }
            return null;
        }

        public List<Message> GetMessagsForVirtualNumber(int virtualNumberId)
        {
            try
            {
                return _context.Messages.Include(m => m.MMSFiles).Where(x => x.VirtualNumberId == virtualNumberId && x.DeleteFlag == null).OrderByDescending(x => x.TimeStamp).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("MessagesDA.GetMessagsForVirtualNumber", ex);
            }
            return null;
        }

        public Message GetMessageByGatewayMessageId(string gatewayMessageId)
        {
            try
            {
                Message msg = _context.Messages.FirstOrDefault(x => x.GatewayMessageId == gatewayMessageId);
                // Get the virtual number.
                if (msg != null && msg.VirtualNumberId > 0)
                {
                    DedicatedVirtualNumber dvn = _context.DedicatedVirtualNumbers.FirstOrDefault(x => x.VirtualNumberId == msg.VirtualNumberId);
                    if (dvn != null)
                    {
                        msg.VirtualNumber = dvn.VirtualNumber;
                    }
                }
                return msg;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("MessagesDA.GetMessageByGatewayMessageId", ex);
            }
            return null;
        }

        public List<Message> GetMessagesForAccountAndRecipient(int accountId, int virtualNumberId, string number)
        {
            try
            {
                return _context.Messages.Include(m => m.MMSFiles)
                    .Where(m => m.AccountId == accountId
                        && m.VirtualNumberId == virtualNumberId
                        && m.MobileNumber == number
                        && m.MessageType != (byte)MessageTypes.Notification
                        && m.DeleteFlag == null).OrderBy(x => x.TimeStamp).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("MessagesDA.GetRecentMessagesForAccountAndRecipient", ex);
            }
            return null;
        }

        public List<Recent> GetRecentToNumbersForDedicatedVirtualNumber(int accountId, int virtualNumberId)
        {
            try
            {
                var query = from dvn in _context.DedicatedVirtualNumbers
                            join msg in _context.Messages on dvn.VirtualNumberId equals msg.VirtualNumberId
                            where dvn.AccountId == accountId && dvn.VirtualNumberId == virtualNumberId && msg.DeleteFlag == null && msg.MessageType != (byte)MessageTypes.Notification
                            group msg by msg.MobileNumber into numGroup
                            select new
                            {
                                Message = numGroup.Select(x => new Recent()
                                {
                                    Number = numGroup.Key,
                                    MessageId = x.MessageId,
                                    TimeStamp = x.TimeStamp,
                                    Message = x.MessageText,
                                    IsActiveMessage = false
                                }).OrderByDescending(x => x.TimeStamp).FirstOrDefault()
                            };

                return query.OrderByDescending(x => x.Message.TimeStamp).Select(x => x.Message).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("MessagesDA.GetRecentMessagesForAccountAndVirtualNumber", ex);
            }
            return null;
        }

        public List<Recent> GetRecentMessagesForAccountAndVirtualNumber(int accountId, int virtualNumberId)
        {
            try
            {
                var query = from m in _context.Messages
                            where m.AccountId == accountId && m.VirtualNumberId == virtualNumberId && m.DeleteFlag == null && m.MessageType != (byte)MessageTypes.Notification
                            group m by m.MobileNumber into numGroup
                            select new
                            {
                                Message = numGroup.Select(x => new Recent()
                                {
                                    Number = numGroup.Key,
                                    MessageId = x.MessageId,
                                    TimeStamp = x.TimeStamp,
                                    Message = x.MessageText,
                                    IsActiveMessage = false
                                }).OrderByDescending(x => x.TimeStamp).FirstOrDefault()
                            };

                return query.OrderByDescending(x => x.Message.TimeStamp).Select(x => x.Message).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("MessagesDA.GetRecentMessagesForAccountAndVirtualNumber", ex);
            }
            return null;
        }

        public InboxContainer GetInboundMessagesForAccount(int accountId, int page, int pageSize, bool getPageCount)
        {
            try
            {
                InboxContainer inboxContainer = new InboxContainer();
                inboxContainer.CurrentPage = page;
                inboxContainer.PageSize = pageSize;

                if (getPageCount)
                {
                    inboxContainer.MessageCount = (from msg in _context.Messages
                                                   join vn in _context.DedicatedVirtualNumbers on msg.VirtualNumberId equals vn.VirtualNumberId
                                                   where vn.AccountId == accountId && msg.Direction == 1 && msg.DeleteFlag == null && vn.Cancelled == false
                                                   select msg.MessageId).Count();

                    if (inboxContainer.MessageCount > 200)
                    {
                        inboxContainer.MessageCount = 200;
                    }
                };

                inboxContainer.Messages = (from msg in _context.Messages
                                           join vn in _context.DedicatedVirtualNumbers on msg.VirtualNumberId equals vn.VirtualNumberId
                                           where vn.AccountId == accountId && msg.Direction == 1 && msg.DeleteFlag == null && vn.Cancelled == false
                                           orderby msg.TimeStamp descending
                                           select new InboxMessage()
                                           {
                                               VirtualNumber = vn.VirtualNumber,
                                               MobileNumber = msg.MobileNumber,
                                               TimeStamp = msg.TimeStamp,
                                               MessageText = msg.MessageText
                                           }).Take(200).Skip(page * pageSize).Take(pageSize).ToList();


                if (inboxContainer.MessageCount > 0 && inboxContainer.PageSize > 0)
                {
                    inboxContainer.PageCount = inboxContainer.MessageCount / inboxContainer.PageSize;
                }
                return inboxContainer;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("MessagesDA.GetInboundMessagesForAccount", ex);
            }
            return null;
        }

        public DedicatedVirtualNumber GetVirtualNumberByNumber(string virtualNumber, bool getActiveNumbersOnly)
        {
            try
            {
                if (getActiveNumbersOnly)
                {
                    return _context.DedicatedVirtualNumbers.FirstOrDefault(x => x.VirtualNumber == virtualNumber && x.Cancelled == false);
                }
                else
                {
                    return _context.DedicatedVirtualNumbers.FirstOrDefault(x => x.VirtualNumber == virtualNumber);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("MessagesDA.GetAccountIdByVirtualNumber", ex);
            }
            return null;
        }

        public DedicatedVirtualNumber GetVirtualNumberByNumberAndOriginatingMobileNumber(string virtualNumber, string mobileNumber)
        {
            try
            {
                // This method is used to match pooled numbers to the accounts that messages originated from.
                // A match is made by matching the virtual number of an inbound message to a virtual number from 
                // the list of pooled numbers, then also finding an existing message that was previously sent that
                // matches the senders number of an inbound message.
                var query = from dvn in _context.DedicatedVirtualNumbers.AsQueryable()
                            join msg in _context.Messages on dvn.VirtualNumberId equals msg.VirtualNumberId
                            where dvn.NumberType == (byte)NumberTypes.Pooled &&
                                dvn.VirtualNumber == virtualNumber && msg.MobileNumber == mobileNumber &&
                                msg.Direction == (byte)MessageDirection.Outbound &&
                                msg.MessageType != (byte)MessageTypes.Notification
                            orderby
                                msg.MessageId descending
                            select new DedicatedVirtualNumber()
                            {
                                AccountId = dvn.AccountId,
                                VirtualNumber = dvn.VirtualNumber,
                                VirtualNumberId = dvn.VirtualNumberId,
                                NumberType = dvn.NumberType
                            };

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("MessagesDA.GetVirtualNumberByNumberAndOriginatingMobileNumber", ex);
            }
            return null;
        }

        public bool NumberIsBlocked(string mobileNumber)
        {
            BlockedNumber bn = _context.BlockedNumbers.FirstOrDefault(x => x.MobileNumber == mobileNumber);
            return (bn != null);
        }

        #endregion

        #region "Update Methods"

        public bool UpdateMessageWithGatewayMessageId(int messageId, string gatewayMessageId, decimal price, string processingMessage)
        {
            try
            {
                Message message = _context.Messages.FirstOrDefault(x => x.MessageId == messageId);
                if (message != null)
                {
                    message.GatewayMessageId = gatewayMessageId;
                    message.Price = price;
                    message.ProcessingMessage += processingMessage;

                    this.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("MessagesDA.UpdateMessageWithGatewayMessageId", ex);
            }

            return false;
        }

        #endregion

        #region "Insert Methods"

        public int InsertMessage(Message message, ref decimal newBalance)
        {
            newBalance = 0;
            try
            {
                Account acc = _context.Accounts.FirstOrDefault(x => x.AccountId == message.AccountId);
                message.MobileNumber = Utilities.NumberToE164(message.MobileNumber);

                // Check and update the balance if the message is an outbound message
                if (message.Direction == (int)MessageDirection.Outbound)
                {
                    if (acc.Balance > 0)
                    {
                        _context.Messages.Add(message);
                        _context.SaveChanges();

                        // Get the customer's messge cost and update the account balance
                        if (message.MMSFiles.Count > 0)
                        {
                            message.CustomerCost = acc.MMSSegmentCost;
                        }
                        else
                        {
                            message.CustomerCost = acc.SMSSegmentCost;
                        }

                        acc.Balance -= (decimal)message.CustomerCost;
                        newBalance = acc.Balance;
                        _context.SaveChanges();

                        return message.MessageId;
                    }
                    else
                    {
                        acc.Balance = -0.0101M;
                        newBalance = acc.Balance;
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("MessagesDA.InsertMessage", ex);
            }
            return 0;
        }

        #endregion

        #region "Delete Methods"

        public int DeleteMessagesForVirtualNumber(int accountId, int virtualNumberId)
        {
            int messagesDeleted = 0;
            try
            {
                List<Message> messagesToDelete = _context.Messages.Where(x => x.AccountId == accountId && x.VirtualNumberId == virtualNumberId && x.DeleteFlag == null).ToList();
                messagesDeleted = messagesToDelete.Count();
                messagesToDelete.ForEach(x => x.DeleteFlag = DateTime.Now);
                SaveChanges();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("MessagesDA.DeleteMessagesForVirtualNumber", ex);
            }

            return messagesDeleted;
        }

        public int DeleteMessagesForVirtualNumberAndMobileNumber(int accountId, int virtualNumberId, string mobileNumber)
        {
            int messagesDeleted = 0;
            try
            {
                List<Message> messagesToDelete = _context.Messages.Where(x => x.AccountId == accountId && x.VirtualNumberId == virtualNumberId && x.MobileNumber == mobileNumber && x.DeleteFlag == null).ToList();
                messagesDeleted = messagesToDelete.Count();
                messagesToDelete.ForEach(x => x.DeleteFlag = DateTime.Now);
                SaveChanges();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("MessagesDA.DeleteMessagesForAccountAndMobileNumber", ex);
            }

            return messagesDeleted;
        }

        #endregion

    }
}