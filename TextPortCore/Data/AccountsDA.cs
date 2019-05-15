﻿using System;
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
                ErrorHandling eh = new ErrorHandling(_context);
                eh.LogException("AccountsDA.GetAccountById", ex);
            }
            return null;
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
                ErrorHandling eh = new ErrorHandling(_context);
                eh.LogException("AccountsDA.GetAccountById", ex);
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
                ErrorHandling eh = new ErrorHandling(_context);
                eh.LogException("AccountDA.IsUsernameAvailable", ex);
            }

            return false;
        }

        public bool IsEmailAvailable(string email)
        {
            try
            {
                return (_context.Accounts.FirstOrDefault(x => x.Email == email) == null) ? true : false;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling(_context);
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
                ErrorHandling eh = new ErrorHandling(_context);
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
                        dbRecord.NotificationsEmailAddress = account.NotificationsEmailAddress;
                        dbRecord.TimeZoneId = account.TimeZoneId;
                        dbRecord.Email = account.Email;

                        //_context.Accounts.Update(dbRecord);
                        _context.SaveChanges();

                        return true;

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling(_context);
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
                    Account acc = new Account() { AccountId = account.AccountId };
                    _context.Accounts.Attach(acc);

                    acc.LoginCount++;
                    acc.LastLogin = DateTime.UtcNow.AddHours(GetTimeZoneOffsetHours(account.TimeZoneId));

                    _context.Entry(acc).Property(x => x.LastLogin).IsModified = true;
                    _context.Entry(acc).Property(x => x.LoginCount).IsModified = true;

                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling(_context);
                eh.LogException("AccountDA.UpdateLastLoginAndLoginCount", ex);
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
                newAccount.Credits = 0;
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
                ErrorHandling eh = new ErrorHandling(_context);
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
                temporaryAccount.Credits = 0;
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
                ErrorHandling eh = new ErrorHandling(_context);
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
                ErrorHandling eh = new ErrorHandling(_context);
                eh.LogException("AccountDA.EnableTemporaryAccount", ex);
            }

            return false;
        }

        #endregion
    }
}
