using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;

using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPortCore.Data
{
    public partial class TextPortDA
    {

        #region "Select Methods"

        public int CheckFreeTextCountForIP(string ipAddress, int limit)
        {
            try
            {
                FreeTextIPAddress addr = _context.FreeTextIPAddresses.Where(x => x.IPAddress == ipAddress).FirstOrDefault();
                if (addr != null)
                {
                    if (addr.RequestCount <= limit)
                    {
                        addr.RequestCount++;
                        this.SaveChanges();
                    }
                    return addr.RequestCount;
                }
                else
                {
                    _context.FreeTextIPAddresses.Add(new FreeTextIPAddress(ipAddress));
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("MiscDA.CheckFreeTextCountForIP", ex);
            }
            return 0;
        }

        public int CheckFreeSendCountForNumber(int accountId, string mobileNumber)
        {
            try
            {
                return _context.Messages.Where(x => x.AccountId == accountId && x.MessageType == (int)MessageTypes.FreeTextSend && x.MobileNumber == mobileNumber && x.Direction == 0 && x.TimeStamp >= DateTime.UtcNow.AddHours(-24)).Count();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("MiscDA.CheckFreeSendCountForNumber", ex);
            }
            return 0;
        }

        #endregion

        #region "Insert Methods"

        public bool AddNumberBlock(BlockRequest blockRequest)
        {
            try
            {
                BlockedNumber existingBlock = _context.BlockedNumbers.FirstOrDefault(x => x.MobileNumber == blockRequest.MobileNumberE164 && x.Direction == (byte)blockRequest.Direction);
                if (existingBlock != null)
                {
                    blockRequest.SubmissionStatus = RequestStatus.Failed;
                    blockRequest.SubmissionMessage = $"The number {blockRequest.MobileNumber} is already on the block list.";
                    return false;
                }

                BlockedNumber blockedNumber = new BlockedNumber(blockRequest);

                _context.BlockedNumbers.Add(blockedNumber);
                _context.SaveChanges();

                blockRequest.BlockId = blockedNumber.BlockID;

                if (blockRequest.BlockId > 0)
                {
                    blockRequest.SubmissionStatus = RequestStatus.Success;
                    blockRequest.SubmissionMessage = $"The number {blockRequest.MobileNumber} was successfully added to the block list.";
                    return true;
                }
                else
                {
                    blockRequest.SubmissionStatus = RequestStatus.Failed;
                    blockRequest.SubmissionMessage = $"An error occurred while attempting to add {blockRequest.MobileNumber} to the block list.";
                    return false;
                }

            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("MiscDA.AddNumberBlock", ex);
            }

            return false;
        }

        #endregion

        #region "Delete Methods"
        #endregion

    }
}