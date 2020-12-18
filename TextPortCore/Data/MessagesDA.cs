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
                return _context.Messages.Include(m => m.DedicatedVirtualNumber).Include(m => m.MMSFiles).Where(x => x.MessageId == messageId).FirstOrDefault();
                //.ThenInclude(m => m.Carrier)
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
                List<Message> messages = _context.Messages.Include(m => m.MMSFiles).Include(x => x.Account).Where(x => x.VirtualNumberId == virtualNumberId && x.DeleteFlag == null).OrderByDescending(x => x.TimeStamp).ToList();

                foreach (Message m in messages)
                {
                    m.ConvertTimeStampToLocalTimeZone();
                }
                return messages;
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
                return _context.Messages.Include(x => x.Account).Include(x => x.DedicatedVirtualNumber).FirstOrDefault(x => x.GatewayMessageId == gatewayMessageId);
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
                List<Message> messages = _context.Messages.Include(m => m.MMSFiles).Include(m => m.Account).Include(m => m.Contact)
                    .Where(m => m.AccountId == accountId
                        && m.VirtualNumberId == virtualNumberId
                        && m.MobileNumber == number
                        && m.MessageType != (byte)MessageTypes.Notification
                        && m.DeleteFlag == null).OrderByDescending(x => x.TimeStamp).Take(300).AsEnumerable().Reverse().ToList();

                foreach (Message m in messages)
                {
                    m.ConvertTimeStampToLocalTimeZone();
                }
                return messages;
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
                var query = from msg in _context.Messages
                            join dvn in _context.DedicatedVirtualNumbers on msg.VirtualNumberId equals dvn.VirtualNumberId
                            join acc in _context.Accounts on msg.AccountId equals acc.AccountId
                            join ctc in _context.Contacts on msg.ContactId equals ctc.ContactId into contacts
                            from contact in contacts.DefaultIfEmpty()
                            where dvn.AccountId == accountId && dvn.VirtualNumberId == virtualNumberId && msg.DeleteFlag == null && msg.MessageType != (byte)MessageTypes.Notification
                            group new { msg.MessageId, msg.TimeStamp, msg.MessageText, dvn.CountryId, acc.TimeZoneId, contact.ContactId, contact.Name }
                            by msg.MobileNumber into numGroup
                            select new
                            {
                                Message = numGroup.Select(x => new Recent()
                                {
                                    Number = numGroup.Key,
                                    ContactId = x.ContactId ?? 0,
                                    ContactName = x.Name,
                                    CountryId = x.CountryId,
                                    MessageId = x.MessageId,
                                    TimeStamp = TimeFunctions.GetUsersLocalTime(x.TimeStamp, x.TimeZoneId),
                                    Message = x.MessageText,
                                    IsActiveMessage = false
                                }).OrderByDescending(x => x.TimeStamp).FirstOrDefault()
                            };

                return query.OrderByDescending(x => x.Message.TimeStamp).Select(x => x.Message).Take(300).ToList();
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
                var query = from msg in _context.Messages
                            join dvn in _context.DedicatedVirtualNumbers on msg.VirtualNumberId equals dvn.VirtualNumberId
                            join acc in _context.Accounts on msg.AccountId equals acc.AccountId
                            join ctc in _context.Contacts on msg.ContactId equals ctc.ContactId into contacts
                            from contact in contacts.DefaultIfEmpty()
                            where msg.AccountId == accountId && msg.VirtualNumberId == virtualNumberId && msg.DeleteFlag == null && msg.MessageType != (byte)MessageTypes.Notification
                            group new { msg.MessageId, msg.TimeStamp, msg.MessageText, dvn.CountryId, acc.TimeZoneId, contact.ContactId, contact.Name }
                            by msg.MobileNumber into numGroup
                            select new
                            {
                                Message = numGroup.Select(x => new Recent()
                                {
                                    Number = numGroup.Key,
                                    ContactId = x.ContactId ?? 0,
                                    ContactName = x.Name,
                                    CountryId = x.CountryId,
                                    MessageId = x.MessageId,
                                    TimeStamp = TimeFunctions.GetUsersLocalTime(x.TimeStamp, x.TimeZoneId),
                                    Message = x.MessageText,
                                    IsActiveMessage = false
                                }).OrderByDescending(x => x.TimeStamp).FirstOrDefault()
                            };

                if (query != null)
                {
                    return query.OrderByDescending(x => x.Message.TimeStamp).Select(x => x.Message).Take(150).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("MessagesDA.GetRecentMessagesForAccountAndVirtualNumber", ex);
            }
            return null;
        }

        public List<Message> GetMessagesForAccountAndSessionId(int accountId, string sessionId)
        {
            try
            {
                List<Message> messages = _context.Messages.Include(m => m.MMSFiles).Include(m => m.Account)
                    .Where(m => m.AccountId == accountId
                        && m.SessionId == sessionId
                        && m.MessageType != (byte)MessageTypes.Notification
                        && m.DeleteFlag == null).OrderByDescending(x => x.TimeStamp).Take(300).AsEnumerable().Reverse().ToList();

                foreach (Message m in messages)
                {
                    m.ConvertTimeStampToLocalTimeZone();
                }
                return messages;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("MessagesDA.GetMessagesForAccountAndSessionId", ex);
            }
            return null;
        }

        public InboxContainer GetInboundMessagesForAccount(int accountId, PagingParameters pParams)
        {
            try
            {
                Account acc = _context.Accounts.FirstOrDefault(x => x.AccountId == accountId);

                int page = pParams.Page;
                int recordsPerPage = pParams.RecordsPerPage;
                int previousRecordsPerPage = pParams.PreviousRecordsPerPage;
                string sortBy = pParams.SortBy;
                string sortOrder = pParams.SortOrder;
                byte filterBy = pParams.Filter;
                byte prevFilterBy = pParams.PrevFilter;

                if (pParams.Operation == "sort")
                {
                    if (pParams.SortBy == pParams.PrevSortBy)
                    {
                        sortOrder = (sortOrder == "asc") ? "desc" : "asc";
                    }
                }
                bool sortDescending = (sortOrder == "desc");

                InboxContainer inboxContainer = new InboxContainer()
                {
                    RecordsPerPage = recordsPerPage,
                    SortOrder = sortOrder
                };

                var msgCount = _context.Messages
                   .Join(_context.DedicatedVirtualNumbers, msg => msg.VirtualNumberId, dvn => dvn.VirtualNumberId, (msg, dvn) => new { Msg = msg, Dvn = dvn })
                   .Where(mv => mv.Msg.AccountId == accountId && mv.Msg.DeleteFlag == null && mv.Dvn.Cancelled == false);
                switch (filterBy)
                {
                    case 0:
                    case 1:
                        msgCount = msgCount.Where(m => m.Msg.Direction == filterBy);
                        break;
                        // Case 2: Don't filter. Show all messages.
                };
                inboxContainer.RecordCount = msgCount.Count();

                if ((previousRecordsPerPage > 0 && recordsPerPage != previousRecordsPerPage) || (prevFilterBy != filterBy))
                {
                    int firstRecord = page * previousRecordsPerPage;
                    if (firstRecord > inboxContainer.RecordCount)
                    {
                        firstRecord = inboxContainer.RecordCount;
                    }

                    for (int x = 1; x <= firstRecord; x++)
                    {
                        if (x * recordsPerPage >= firstRecord)
                        {
                            page = x;
                            x += firstRecord;
                        }
                    }
                }
                inboxContainer.CurrentPage = page;
                inboxContainer.LowRecord = (recordsPerPage * (page - 1)) + 1;
                inboxContainer.HighRecord = inboxContainer.LowRecord + recordsPerPage - 1;

                var messages = _context.Messages
                     .Join(_context.DedicatedVirtualNumbers, msg => msg.VirtualNumberId, dvn => dvn.VirtualNumberId, (msg, dvn) => new { Msg = msg, Dvn = dvn })
                     .Where(mv => mv.Msg.AccountId == accountId && mv.Msg.DeleteFlag == null && mv.Dvn.Cancelled == false);
                switch (filterBy)
                {
                    case 0:
                    case 1:
                        messages = messages.Where(m => m.Msg.Direction == filterBy);
                        break;
                        // Case 2: Don't filter. Show all messages.
                }
                inboxContainer.Messages = messages.Select(mv => new InboxMessage()
                {
                    MessageId = mv.Msg.MessageId,
                    Direction = mv.Msg.Direction,
                    VirtualNumber = mv.Dvn.VirtualNumber,
                    MobileNumber = mv.Msg.MobileNumber,
                    TimeStamp = TimeFunctions.GetUsersLocalTime(mv.Msg.TimeStamp, acc.TimeZoneId),
                    MessageText = mv.Msg.MessageText
                }).OrderBy(pParams.SortBy, sortDescending).Skip((page - 1) * recordsPerPage).Take(recordsPerPage).ToList();

                //var messages = from msg in _context.Messages
                //               where msg.Accoun
                //               join vn in _context.DedicatedVirtualNumbers on msg.VirtualNumberId equals vn.VirtualNumberId;

                //where vn.AccountId ==  && msg.DeleteFlag == null && vn.Cancelled == false;

                //select new InboxMessage()
                //{
                //    MessageId = msg.MessageId,
                //    VirtualNumber = vn.VirtualNumber,
                //    MobileNumber = msg.MobileNumber,
                //    TimeStamp = msg.TimeStamp,
                //    MessageText = msg.MessageText
                //}).OrderBy(pParams.SortBy, sortDescending).Skip((page - 1) * recordsPerPage).Take(recordsPerPage).ToList();
                //inboxContainer.Messages = (from msg in _context.Messages
                //                           join vn in _context.DedicatedVirtualNumbers on msg.VirtualNumberId equals vn.VirtualNumberId
                //                           where vn.AccountId == accountId && msg.Direction == 1 && msg.DeleteFlag == null && vn.Cancelled == false
                //                           select new InboxMessage()
                //                           {
                //                               MessageId = msg.MessageId,
                //                               VirtualNumber = vn.VirtualNumber,
                //                               MobileNumber = msg.MobileNumber,
                //                               TimeStamp = msg.TimeStamp,
                //                               MessageText = msg.MessageText
                //                           }).OrderBy(pParams.SortBy, sortDescending).Skip((page - 1) * recordsPerPage).Take(recordsPerPage).ToList();

                if (inboxContainer.HighRecord > inboxContainer.RecordCount)
                {
                    inboxContainer.HighRecord = inboxContainer.RecordCount;
                }

                if (inboxContainer.RecordCount > 0 && inboxContainer.RecordsPerPage > 0)
                {
                    float pageCount = (float)inboxContainer.RecordCount / (float)inboxContainer.RecordsPerPage;
                    inboxContainer.PageCount = (int)pageCount;
                    if (pageCount - inboxContainer.PageCount > 0)
                    {
                        inboxContainer.PageCount++;
                    }
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
                    return _context.DedicatedVirtualNumbers.Include(x => x.Account).FirstOrDefault(x => x.VirtualNumber == virtualNumber && x.Cancelled == false);
                }
                else
                {
                    return _context.DedicatedVirtualNumbers.Include(x => x.Account).FirstOrDefault(x => x.VirtualNumber == virtualNumber);
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
                            select dvn;

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("MessagesDA.GetVirtualNumberByNumberAndOriginatingMobileNumber", ex);
            }
            return null;
        }

        public Message GetOriginatingMessageByVirtualNumberIdAndMobileNumberAndMessageType(int virtualNumberId, string mobileNumber, MessageTypes messageType)
        {
            try
            {
                // This method is used to locate the originating (outbound) message sent from a virtual
                // number to a specified mobile number for the given message type.
                return _context.Messages.Include(x => x.DedicatedVirtualNumber).ThenInclude(x => x.Account).Where(x => x.VirtualNumberId == virtualNumberId &&
                                        x.MessageType == (byte)messageType &&
                                        x.Direction == (byte)MessageDirection.Outbound &&
                                        x.MobileNumber == mobileNumber).OrderByDescending(x => x.MessageId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("MessagesDA.GetOriginatingMessageByVirtualNumberIdAndMobileNumberAndMessageType", ex);
            }
            return null;
        }

        public string GetOriginalSMSToEmailSenderAddressByAccountIdVirtualNumberIdAndMobileNumber(int accountId, int virtualNumberId, string mobileNumber)
        {
            try
            {
                // This method is used to match an SMS to Email Address to an existing EMail-to-SMS outbound message that was sent from that address.
                // When a message is received a lookup is performed on the receiving virtual number ID, the receiving accountId (probably redundant),
                // and the mobile number that the message was originally send to. The original outbound message will have the EMailToSMSAddressId set
                // which can be tied back to the original sender. This allows inbound messages to be directed back to the original sender of the 
                // email-to-sms message.
                var query = from
                    msg in _context.Messages.AsQueryable()
                            join
                                emlAdd in _context.EmailToSMSAddresses on msg.EmailToSMSAddressId equals emlAdd.AddressId
                            where
                                msg.AccountId == accountId &&
                                msg.VirtualNumberId == virtualNumberId &&
                                msg.MobileNumber == mobileNumber &&
                                msg.Direction == (byte)MessageDirection.Outbound &&
                                msg.MessageType == (byte)MessageTypes.EmailToSMS
                            orderby
                                msg.MessageId descending
                            select
                                emlAdd.EmailAddress;

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("MessagesDA.GetOriginalSMSToEmailSenderAddressByAccountIdVirtualNumberIdAndMobileNumber", ex);
            }
            return null;
        }

        public bool NumberIsBlocked(string mobileNumber, MessageDirection direction)
        {
            mobileNumber = Utilities.NumberToE164(mobileNumber, "1");
            BlockedNumber bn = _context.BlockedNumbers.FirstOrDefault(x => x.MobileNumber == mobileNumber && x.Direction == (byte)direction);
            if (bn != null)
            {
                bn.BlockCount++;
                SaveChanges();

                return true;
            }
            return false;
        }

        #endregion

        #region "Update Methods"

        public bool UpdateMessageWithGatewayMessageId(int messageId, string gatewayMessageId, int segmentCount, QueueStatuses status, string processingMessage)
        {
            try
            {
                Message message = _context.Messages.FirstOrDefault(x => x.MessageId == messageId);
                if (message != null)
                {
                    message.GatewayMessageId = gatewayMessageId;
                    message.Segments = segmentCount;
                    message.QueueStatus = (byte)status;
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

        public int FlagListOfMessagesAsDeleted(int accountId, MessageIdList messageIds)
        {
            int messagesDeleted = 0;
            try
            {
                List<int> ids = new List<int>();
                foreach (MessageIdItem idItem in messageIds.Ids)
                {
                    ids.Add(idItem.Id);
                }

                var messagesToDelete = _context.Messages.Where(x => x.AccountId == accountId && ids.Contains(x.MessageId)).ToList();
                messagesToDelete.ForEach(x => x.DeleteFlag = DateTime.UtcNow);
                messagesDeleted = SaveChanges();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("MessagesDA.FlagListOfMessagesAsDeleted", ex);
            }

            return messagesDeleted;
        }

        #endregion

        #region "Insert Methods"

        public int InsertMessage(Message message, ref decimal newBalance)
        {
            newBalance = 0;
            try
            {
                if (message.Account == null)
                {
                    Account acc = _context.Accounts.Include(a => a.TimeZone).FirstOrDefault(x => x.AccountId == message.AccountId);
                    message.Account = acc;
                }

                // Check for a contact association
                if (message.ContactId == null || message.ContactId == 0)
                {
                    int? contactId = _context.Contacts.FirstOrDefault(x => x.AccountId == message.AccountId && x.MobileNumber == message.MobileNumber)?.ContactId;
                    message.ContactId = (contactId != null) ? contactId : 0;
                }

                message.IsMMS = (message.MMSFiles?.Count > 0);

                // Only insert the message, inbound or outbound if the account has a balance.
                if (message.Account.Balance > 0)
                {
                    // Only update deduct the cost from the balance if the message is outbound.
                    if (message.Direction == (int)MessageDirection.Outbound)
                    {
                        if (message.Segments == null) message.Segments = 1;

                        // Deduct the message cost from the balance now. If the message fails, the cost can be credited back later once a response is received from the provider.
                        message.CustomerCost = (message.IsMMS) ? message.Account.MMSSegmentCost : message.Account.SMSSegmentCost;
                        message.CustomerCost *= message.Segments;

                        message.Account.MessageOutCount++;
                        message.Account.Balance -= (decimal)message.CustomerCost;
                        newBalance = message.Account.Balance;
                    }

                    _context.Messages.Add(message);
                    _context.SaveChanges();

                    return message.MessageId;
                }
                else
                {
                    message.Account.Balance = 0M;
                    message.ProcessingMessage = "Insufficient balance";
                    _context.SaveChanges();
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