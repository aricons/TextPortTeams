using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Script.Serialization;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;
using TextPortCore.Integrations.Bandwidth;
using TextPortServices.Processes;
using EmailToSMSGateway;

namespace Testing
{
    [TestClass]
    public class ServicesTests
    {
        [TestMethod]
        public void TestCheckForVirtualNumberExpirations()
        {
            int days = 7;
            VnExpirationsPoller.CheckForVirtualNumberExpirationNotifications(days, NumberTypes.Regular);

            int foo = days;
        }

        [TestMethod]
        public void TestCheckForPooledVirtualNumberExpirations()
        {
            int days = 2;
            VnExpirationsPoller.CheckForVirtualNumberExpirationNotifications(days, NumberTypes.Pooled);

            int foo = days;
        }

        [TestMethod]
        public void TestCheckForAutoRenewNumberLowBalanceExpirations()
        {
            int days = 7;
            VnExpirationsPoller.CheckForAutoRenewNumberExpirtionLowBalanceNotifications(days, NumberTypes.Regular);

            int foo = days;
        }

        [TestMethod]
        public void TestCheckForAndCancelExpiredNumbers()
        {
            VnExpirationsPoller.CheckForAndCancelExpiredNumbers();

            int foo = 2;
        }

        [TestMethod]
        public void CheckForAndRenewAutoRenewNumbers()
        {
            VnExpirationsPoller.CheckForAndRenewAutoRenewNumbers();

            int foo = 2;
        }

        [TestMethod]
        public void TestCheckForAndCancelAutoRenewNumbersWithInsufficientBalance()
        {
            VnExpirationsPoller.CheckForAndCancelAutoRenewNumbersWithInsufficientBalance();

            int foo = 2;
        }
    }
}