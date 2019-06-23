using System;
using System.Collections.Generic;
using System.Linq;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPort.WebServices
{
    public static class WebServicesCommon
    {
        // Public methods
        public static TextPortSMSResponses ProcessMessageRequests(TextPortSMSMessages messagesList, MessageTypes messageType)
        {
            TextPortSMSResponses responseList = new TextPortSMSResponses() { Responses = new List<TextPortSMSResponse>() };
            string validationMessage = String.Empty;
            string requestorIpAddress = Utilities.GetUserHostAddress();

            int accountID = 0;
            int itemCount = 0;
            int virtualNumberId = 0;
            decimal balance = 0;

            if (messagesList != null)
            {
                if (!String.IsNullOrEmpty(messagesList.UserName) && !String.IsNullOrEmpty(messagesList.Password))
                {
                    using (TextPortDA da = new TextPortDA())
                    {
                        if (verifyCredentials(da, messagesList.UserName, messagesList.Password, ref accountID, ref virtualNumberId, ref balance))
                        {
                            if (balance > 0)
                            {
                                if (virtualNumberId > 0)
                                {
                                    if (messagesList.Messages != null)
                                    {
                                        if (messagesList.Messages.Count > 0)
                                        {
                                            foreach (TextPortSMSMessage messageRequest in messagesList.Messages)
                                            {
                                                itemCount++;
                                                TextPortSMSResponse responseItem = new TextPortSMSResponse()
                                                {
                                                    ItemNumber = itemCount,
                                                    MobileNumber = Utilities.NumberToE164(messageRequest.MobileNumber),
                                                    ErrorMessage = String.Empty,
                                                    ProcessingMessage = "Validating message request " + itemCount.ToString() + ". ",
                                                    Result = "Unknown",
                                                    MessageID = 0
                                                };

                                                if (validateMessage(messageRequest, ref validationMessage))
                                                {
                                                    responseItem.ProcessingMessage += "Validation completed successfully. ";

                                                    //SelectCarrierIDForCountryAbbrevResult carrierResult = getCarrierIDForCountryAbbreviation(messageRequest.CountryCode);
                                                    Message message = new Message()
                                                    {
                                                        AccountId = accountID,
                                                        MessageType = (byte)messageType,
                                                        Direction = (byte)MessageDirection.Outbound,
                                                        MobileNumber = Utilities.NumberToE164(messageRequest.MobileNumber),
                                                        MessageText = messageRequest.MessageText,
                                                        Ipaddress = requestorIpAddress,
                                                        CarrierId = (int)Carriers.BandWidth,
                                                        IsMMS = false,
                                                        TimeStamp = DateTime.UtcNow,
                                                        VirtualNumberId = virtualNumberId
                                                    };

                                                    int messageId = 0;
                                                    decimal newBalance = 0;
                                                    messageId = da.InsertMessage(message, ref newBalance);
                                                    if (messageId > 0)
                                                    {
                                                        if (message.Send())
                                                        {
                                                            responseItem.Result = "Success";
                                                            responseItem.ProcessingMessage += "Message processed successfully.";
                                                            responseItem.MessageID = message.MessageId;
                                                        }
                                                        else
                                                        {
                                                            responseItem.Result = "Failed";
                                                            responseItem.ProcessingMessage += "Message processing failed. " + message.ProcessingMessage;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        responseItem.Result = "Failed";
                                                        responseItem.ProcessingMessage += message.ProcessingMessage;
                                                    }
                                                }
                                                else
                                                {
                                                    responseItem.ProcessingMessage += "Message validation failed. Refer to error message for details";
                                                    responseItem.ErrorMessage += validationMessage;
                                                    responseItem.Result = "Validation failure";
                                                }
                                                responseList.Responses.Add(responseItem);
                                            }
                                        }
                                        else
                                        {
                                            responseList.Responses.Add(new TextPortSMSResponse()
                                            {
                                                MobileNumber = String.Empty,
                                                ErrorMessage = "Nothing to process",
                                                ProcessingMessage = "No messages were submitted with the request",
                                                Result = "Failed",
                                                MessageID = 0
                                            });
                                        }
                                    }
                                    else
                                    {
                                        responseList.Responses.Add(new TextPortSMSResponse()
                                        {
                                            MobileNumber = String.Empty,
                                            ErrorMessage = "Nothing to process",
                                            ProcessingMessage = "No message list was submitted with the request",
                                            Result = "Failed",
                                            MessageID = 0
                                        });
                                    }
                                }
                                else
                                {
                                    responseList.Responses.Add(new TextPortSMSResponse()
                                    {
                                        MobileNumber = String.Empty,
                                        ErrorMessage = "There are no active virtual numbers on this account",
                                        ProcessingMessage = "No active virtual numbers",
                                        Result = "Failed",
                                        MessageID = 0
                                    });
                                }
                            }
                            else
                            {
                                responseList.Responses.Add(new TextPortSMSResponse()
                                {
                                    MobileNumber = String.Empty,
                                    ErrorMessage = "Insufficient credits available to process this message",
                                    ProcessingMessage = "Credit balance exhausted",
                                    Result = "Failed",
                                    MessageID = 0
                                });
                            }
                        }
                        else
                        {
                            responseList.Responses.Add(new TextPortSMSResponse()
                            {
                                MobileNumber = String.Empty,
                                ErrorMessage = "Invalid username and/or password",
                                ProcessingMessage = "Authentication failed",
                                Result = "Authentication failure",
                                MessageID = 0
                            });
                        }
                    }
                }
                else
                {
                    responseList.Responses.Add(new TextPortSMSResponse()
                    {
                        MobileNumber = String.Empty,
                        ErrorMessage = "User credentials missing",
                        ProcessingMessage = "A username and/or password was not provided. A valid TextPort username and password must be submitted when consuming the web service. A new account can be set up by visting http://www.textport.com and clicking the 'Register' link. ",
                        Result = "Authentication failure",
                        MessageID = 0
                    });
                }
            }
            else
            {
                responseList.Responses.Add(new TextPortSMSResponse()
                {
                    MobileNumber = String.Empty,
                    ErrorMessage = "Empty request list.",
                    ProcessingMessage = "Empty request list.",
                    Result = "Failed",
                    MessageID = 0
                });
            }
            return responseList;
        }

        public static string ValidateUserCredentials(string userName, string password)
        {
            int accountId = 0;
            int virtualNumberId = 0;
            decimal balance = 0;

            if (!String.IsNullOrEmpty(userName))
            {
                if (!String.IsNullOrEmpty(password))
                {
                    using (TextPortDA da = new TextPortDA())
                    {
                        return (verifyCredentials(da, userName, password, ref accountId, ref virtualNumberId, ref balance)) ? "User authenticated successfully" : "Authentication failed";
                    }
                }
                else
                {
                    return "Password is empty. Both a username and password must be supplied.";
                }
            }
            else
            {
                return "Username is empty. Both a username and password must be supplied.";
            }
        }

        public static int GetCreditBalance(string userName, string password)
        {
            int accountId = 0;
            int virtualNumberId = 0;
            decimal balance = 0;

            if (!String.IsNullOrEmpty(userName) && !String.IsNullOrEmpty(password))
            {
                using (TextPortDA da = new TextPortDA())
                {
                    if (verifyCredentials(da, userName, password, ref accountId, ref virtualNumberId, ref balance))
                    {
                        return Convert.ToInt32(balance);
                    }
                }
            }
            return 0;
        }


        // Private methods
        private static bool verifyCredentials(TextPortDA da, string login, string password, ref int accountId, ref int virtualNumberId, ref decimal balance)
        {
            accountId = 0;
            virtualNumberId = 0;
            balance = 0;

            Account acct = null;
            if (da.ValidateLogin(login, password, ref acct))
            {
                if (acct.AccountId > 0)
                {
                    accountId = acct.AccountId;
                    balance = acct.Balance;

                    List<DedicatedVirtualNumber> numbers = da.GetNumbersForAccount(accountId, false);
                    if (numbers != null)
                    {
                        DedicatedVirtualNumber number = numbers.FirstOrDefault();
                        if (number != null)
                        {
                            virtualNumberId = number.VirtualNumberId;
                            return true;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        //private static int verifyAccountCreditBalance(int accountID)
        //{
        //    return DataAccess.GetCreditBalanceForAccount(accountID);
        //}

        //private static SelectCarrierIDForCountryAbbrevResult getCarrierIDForCountryAbbreviation(string countryAbbreviation)
        //{
        //    try
        //    {
        //        SelectCarrierIDForCountryAbbrevResult result = DataAccess.GetCarrierIDForCountryAbbreviation(countryAbbreviation);

        //        if (result.CarrierID > 0 && !String.IsNullOrEmpty(result.CountryCode))
        //        {
        //            return result;
        //        }
        //        else
        //        {
        //            result.CarrierID = 0;
        //            result.CountryCode = String.Empty;
        //            return result;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return new SelectCarrierIDForCountryAbbrevResult() { CarrierID = 0, CountryCode = String.Empty };
        //    }
        //}

        private static bool validateMessage(TextPortSMSMessage messageRequest, ref string validationMessage)
        {
            validationMessage = String.Empty;

            try
            {
                if (!String.IsNullOrEmpty(messageRequest.CountryCode))
                {
                    if (!String.IsNullOrEmpty(messageRequest.MobileNumber))
                    {
                        if (!String.IsNullOrEmpty(messageRequest.MessageText))
                        {
                            return true;
                        }
                        else
                        {
                            validationMessage += "The message text submitted was empty.  This is a requied value. ";
                            return false;
                        }
                    }
                    else
                    {
                        validationMessage += "The destination mobile number submitted was empty.  This is a requied value. ";
                        return false;
                    }
                }
                else
                {
                    validationMessage += "The destination country code submitted was empty.  This is a requied value. ";
                    return false;
                }
            }
            catch (Exception)
            {
                validationMessage += "An error was encountered validatiing the message request. ";
                return false;
            }
        }
    }
}