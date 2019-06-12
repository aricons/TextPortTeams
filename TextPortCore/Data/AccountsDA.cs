using System;
using System.Linq;

using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPortCore.Data
{
    public partial class TextPortDA
    {
        #region "Select Methods"

        public Account GetAccountById(int accountId)
        {
            try
            {
                return _context.Accounts.FirstOrDefault(x => x.AccountId == accountId);
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("AccountsDA.GetAccountById", ex);
            }
            return null;
        }

        public decimal GetAccountBalance(int accountId)
        {
            try
            {
                return _context.Accounts.FirstOrDefault(x => x.AccountId == accountId).Balance;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("AccountsDA.GetAccountBalance", ex);
            }

            return 0;
        }

        public bool ValidateLogin(string userNameOrEmail, string password, ref Account acc)
        {
            acc = null;
            try
            {
                if (Utilities.IsValidEmail(userNameOrEmail))
                {
                    acc = _context.Accounts.FirstOrDefault(x => x.Email == userNameOrEmail);
                }
                else
                {
                    acc = _context.Accounts.FirstOrDefault(x => x.UserName == userNameOrEmail);
                }

                if (acc != null)
                {
                    string decryptedPassword = AESEncryptDecrypt.Decrypt(acc.Password, Constants.RC4Key);
                    if (password.Equals(decryptedPassword, StringComparison.CurrentCulture))
                    {
                        UpdateLastLoginAndLoginCount(acc);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("AccountsDA.ValidateLogin", ex);
            }

            return false;
        }

        public bool IsUsernameAvailable(string username)
        {
            try
            {
                return (_context.Accounts.FirstOrDefault(x => x.UserName == username && x.Enabled == true) == null) ? true : false;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("AccountDA.IsUsernameAvailable", ex);
            }

            return false;
        }

        public bool IsEmailAvailable(string email)
        {
            try
            {
                return (_context.Accounts.FirstOrDefault(x => x.Email == email && x.Enabled == true) == null) ? true : false;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("AccountDA.IsEmailAvailable", ex);
            }

            return false;
        }

        public int GetTimeZoneOffsetHours(int timeZoneId)
        {
            try
            {
                Models.TimeZone tz = _context.TimeZones.FirstOrDefault(x => x.TimeZoneId == timeZoneId);
                if (tz != null)
                {
                    return Convert.ToInt32(tz.Utcoffset);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("AccountDA.GetTimeZoneOffsetHours", ex);
            }

            return 0;
        }

        #endregion

        #region "Update Methods"

        public bool UpdateAccount(Account account)
        {
            try
            {
                if (account != null)
                {
                    Account dbRecord = _context.Accounts.FirstOrDefault(x => x.AccountId == account.AccountId);
                    if (dbRecord != null)
                    {
                        dbRecord.EnableEmailNotifications = account.EnableEmailNotifications;
                        dbRecord.EnableMobileForwarding = account.EnableMobileForwarding;
                        dbRecord.NotificationsEmailAddress = account.NotificationsEmailAddress;
                        dbRecord.ForwardVnmessagesTo = Utilities.NumberToE164(account.ForwardVnmessagesTo);
                        dbRecord.TimeZoneId = account.TimeZoneId;
                        dbRecord.Email = account.Email;

                        _context.Accounts.Update(dbRecord);
                        this.SaveChanges();

                        return true;

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("AccountsDA.UpdateAccount", ex);
            }

            return false;
        }

        public bool UpdateLastLoginAndLoginCount(Account account)
        {
            try
            {
                if (account != null)
                {
                    account.LoginCount++;
                    account.LastLogin = DateTime.UtcNow.AddHours(GetTimeZoneOffsetHours(account.TimeZoneId));

                    _context.Accounts.Update(account);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("AccountDA.UpdateLastLoginAndLoginCount", ex);
            }

            return false;
        }

        public bool SetComplimentaryNumberFlag(int accountId, byte flagValue)
        {
            try
            {
                Account acc = GetAccountById(accountId);
                if (acc != null)
                {
                    acc.ComplimentaryNumber = flagValue;
                    SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("AccountDA.SetComplimentaryNumberFlag", ex);
            }

            return false;
        }

        #endregion

        #region "Insert Methods"

        public int AddAccount(RegistrationData rd)
        {
            try
            {
                Account newAccount = new Account();

                newAccount.CreateDate = DateTime.UtcNow;
                newAccount.Balance = 0;
                newAccount.Deleted = false;
                newAccount.Email = rd.EmailAddress;
                newAccount.LastLogin = null;
                newAccount.LoginCount = 0;
                newAccount.MessageInCount = 0;
                newAccount.MessageOutCount = 0;
                newAccount.Password = AESEncryptDecrypt.Encrypt(rd.Password, Constants.RC4Key);
                newAccount.TimeZoneId = 5;
                newAccount.UserName = rd.UserName;

                return 1234;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("AccountDA.AddAccount", ex);
            }

            return 0;
        }

        public int AddTemporaryAccount(RegistrationData rd)
        {
            try
            {
                // If a non-enabled account already exists, remove it. 
                Account existingAccount = _context.Accounts.FirstOrDefault(x => x.UserName == rd.UserName && x.Enabled == false);
                if (existingAccount != null)
                {
                    _context.Accounts.Remove(existingAccount);
                }

                // Add the temporary account.
                Account temporaryAccount = new Account();

                temporaryAccount.CreateDate = DateTime.UtcNow;
                temporaryAccount.Balance = 0;
                temporaryAccount.Enabled = false;
                temporaryAccount.Deleted = false;
                temporaryAccount.Email = rd.EmailAddress;
                temporaryAccount.LastLogin = null;
                temporaryAccount.LoginCount = 0;
                temporaryAccount.MessageInCount = 0;
                temporaryAccount.MessageOutCount = 0;
                temporaryAccount.Password = AESEncryptDecrypt.Encrypt(rd.Password, Constants.RC4Key);
                temporaryAccount.TimeZoneId = 5;
                temporaryAccount.UserName = rd.UserName;

                _context.Accounts.Add(temporaryAccount);

                _context.SaveChanges();

                return temporaryAccount.AccountId;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("AccountDA.AddAccount", ex);
            }

            return 0;
        }

        #endregion

        #region "Update Methods"

        public bool EnableTemporaryAccount(RegistrationData rd)
        {
            try
            {
                Account existingAccount = _context.Accounts.FirstOrDefault(x => x.AccountId == rd.AccountId);
                if (existingAccount != null)
                {
                    existingAccount.Enabled = true;
                    _context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("AccountDA.EnableTemporaryAccount", ex);
            }

            return false;
        }

        #endregion
    }
}
