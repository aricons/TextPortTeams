using System;
using System.Collections.Generic;
using System.Linq;

using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPortCore.Data
{
    public partial class TextPortDA
    {

        #region "Select Methods"

        public APIApplication AuthorizeAPI(string token, string secret)
        {
            try
            {
                return _context.APIApplications.FirstOrDefault(x => x.APIToken == token && x.APISecret == secret);
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("ApiDA.AuthorizeAPI", ex);
            }

            return null;
        }

        public List<APIApplication> GetAPIApplicationsForAccount(int accountId)
        {
            try
            {
                return _context.APIApplications.Where(x => x.AccountId == accountId).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("ApiDA.GetAPIApplicationsForAccount", ex);
            }

            return null;
        }

        public APIApplication GetAPIApplicationById(int apiApplicationId)
        {
            try
            {
                return _context.APIApplications.FirstOrDefault(x => x.APIApplicationId == apiApplicationId);
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("ApiDA.GetAPIApplicationsById", ex);
            }

            return null;
        }

        #endregion

        #region "Insert Methods"

        public int SaveAPIApplication(APIApplication apiApp)
        {
            int appId = 0;
            try
            {
                if (apiApp.APIApplicationId == 0)
                {
                    _context.APIApplications.Add(apiApp);
                    _context.SaveChanges();
                    appId = apiApp.APIApplicationId;
                }
                else
                {
                    APIApplication app = GetAPIApplicationById(apiApp.APIApplicationId);
                    if (app != null)
                    {
                        app.ApplicationName = apiApp.ApplicationName;
                        app.APISecret = apiApp.APISecret;
                        app.CallbackURL = apiApp.CallbackURL;
                        app.CallbackUserName = apiApp.CallbackUserName;
                        app.CallbackPassword = apiApp.CallbackPassword;
                        SaveChanges();

                        appId = apiApp.APIApplicationId;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("ApiDA.SaveAPIApplication", ex);
            }
            return appId;
        }

        #endregion

        #region "Update Methods"

        public bool UpdateAPISecret(int apiApplicationId, string newSecretKey)
        {
            try
            {
                if (apiApplicationId > 0)
                {
                    APIApplication app = GetAPIApplicationById(apiApplicationId);
                    if (app != null)
                    {
                        app.APISecret = newSecretKey;
                        SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("ApiDA.UpdateAPISecret", ex);
            }
            return false;
        }

        public bool AssignAPIApplicationToVirtualNumber(int apiApplicationId, int virtualNumberId)
        {
            try
            {
                if (apiApplicationId > 0 && virtualNumberId > 0)
                {
                    DedicatedVirtualNumber dvn = _context.DedicatedVirtualNumbers.FirstOrDefault(x => x.VirtualNumberId == virtualNumberId);
                    if (dvn != null)
                    {
                        dvn.APIApplicationId = apiApplicationId;
                        SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("ApiDA.AssignAPIAppToVirtualNumber", ex);
            }
            return false;
        }

        #endregion

        #region "Delete Methods"

        public bool DeleteAPIApplication(APIApplication apiApp)
        {
            try
            {
                if (apiApp.APIApplicationId > 0)
                {
                    _context.APIApplications.Remove(apiApp);
                    _context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("ApiDA.DeleteAPIApplication", ex);
            }
            return false;
        }

        #endregion

    }
}
