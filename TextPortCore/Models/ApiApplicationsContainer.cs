using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.ComponentModel.DataAnnotations;

using TextPortCore.Data;
using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public class ApiApplicationsContainer
    {
        public int AccountId { get; set; }

        [Display(Name = "Select an Application")]
        public int CurrentApplicationId { get; set; }

        [Display(Name = "Application Name")]
        public APIApplication CurrentApplication { get; set; }

        public IEnumerable<SelectListItem> ApplicationsList { get; set; }

        public string StatusMessage { get; set; }

        public RequestStatus Status { get; set; }

        public int VirtualNumberId { get; set; }

        // Constructors
        public ApiApplicationsContainer()
        {
            this.AccountId = 0;
            this.CurrentApplicationId = 0;
            this.CurrentApplication = new APIApplication();
            this.ApplicationsList = new List<SelectListItem>();
            this.StatusMessage = string.Empty;
            this.Status = RequestStatus.Pending;
            this.VirtualNumberId = 0;
        }

        public ApiApplicationsContainer(int accountId, int currentAppId, int virtualNumberId)
        {
            this.AccountId = accountId;
            this.CurrentApplicationId = 0;
            this.CurrentApplication = new APIApplication();
            this.StatusMessage = string.Empty;
            this.Status = RequestStatus.Pending;
            this.VirtualNumberId = virtualNumberId;

            using (TextPortDA da = new TextPortDA())
            {
                this.ApplicationsList = da.GetAPIApplicationsList(accountId, virtualNumberId);

                if (currentAppId > 0)
                {
                    this.CurrentApplicationId = currentAppId;
                    this.CurrentApplication = da.GetAPIApplicationById(this.CurrentApplicationId);
                    this.StatusMessage = "Application Saved Successfully";
                    this.Status = RequestStatus.Success;
                }
                else
                {
                    if (currentAppId == -1)
                    {
                        this.StatusMessage = "Application Deleted Successfully";
                        this.Status = RequestStatus.Success;
                        this.CurrentApplicationId = 0;
                    };

                    if (this.ApplicationsList.Count() > 1)
                    {
                        this.CurrentApplicationId = Convert.ToInt32(this.ApplicationsList.Skip(1).FirstOrDefault().Value);
                        this.CurrentApplication = da.GetAPIApplicationById(this.CurrentApplicationId);
                    }
                }

                if (ApplicationsList.Count() == 1)
                {
                    CurrentApplication.APIToken = $"{this.AccountId}-{RandomString.GenerateRandomToken(10)}";
                    CurrentApplication.APISecret = RandomString.GenerateRandomToken(20);
                }
            }
        }
    }
}
