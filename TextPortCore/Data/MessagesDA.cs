using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Diagnostics;

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
                Message msg = _context.Messages.FirstOrDefault(x => x.MessageId == messageId);
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
                ErrorHandling eh = new ErrorHandling(_context);
                eh.LogException("MessagesDA.GetMessageById", ex);
            }
            return null;
        }

        public List<Message> GetMessagesForAccountAndRecipient(int accountId, int virtualNumberId, string number)
        {
            try
            {
                return _context.Messages.Where(x => x.AccountId == accountId && x.VirtualNumberId == virtualNumberId && x.MobileNumber == number).OrderBy(x => x.MessageId).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling(_context);
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
                            where dvn.AccountId == accountId && dvn.VirtualNumberId == virtualNumberId //&& msg.Direction == 0
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
                                }).OrderByDescending(x => x.MessageId).FirstOrDefault()
                            };

                return query.OrderByDescending(x => x.Message.TimeStamp).Select(x => x.Message).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling(_context);
                eh.LogException("MessagesDA.GetRecentMessagesForAccountAndVirtualNumber", ex);
            }
            return null;
        }

        public List<Recent> GetRecentMessagesForAccountAndVirtualNumber(int accountId, int virtualNumberId)
        {
            try
            {
                var query = from m in _context.Messages
                            where m.AccountId == accountId && m.VirtualNumberId == virtualNumberId //&& m.Direction == 0
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
                                }).OrderByDescending(x => x.MessageId).FirstOrDefault()
                            };

                return query.OrderByDescending(x => x.Message.TimeStamp).Select(x => x.Message).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling(_context);
                eh.LogException("MessagesDA.GetRecentMessagesForAccountAndVirtualNumber", ex);
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
                ErrorHandling eh = new ErrorHandling(_context);
                eh.LogException("MessagesDA.GetAccountIdByVirtualNumber", ex);
            }
            return null;
        }

        #endregion

        #region "Update Methods"

        //public bool UpdateMessageQueueStatus(Message message)
        //{
        //    try
        //    {
        //        if (message != null)
        //        {
        //            //dbRecord.NotificationsEmailAddress = account.NotificationsEmailAddress;
        //            //dbRecord.TimeZoneId = account.TimeZoneId;
        //            //dbRecord.Email = account.Email;

        //            //_context.Accounts.Update(dbRecord);
        //            _context.SaveChanges();

        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandling eh = new ErrorHandling(_context);
        //        eh.LogException("AccountDA.UpdateLastLoginAndLoginCount", ex);
        //    }

        //    return false;
        //}

        #endregion

        #region "Insert Methods"

        public int InsertMessage(Message message)
        {
            try
            {
                _context.Messages.Add(message);
                _context.SaveChanges();
                return message.MessageId;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling(_context);
                eh.LogException("MessagesDA.InsertMessage", ex);
            }
            return 0;
        }

        #endregion

    }
}