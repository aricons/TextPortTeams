using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;

using Dapper;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;
using TextPortCore.ViewModels;

namespace TextPort.Controllers
{
    public class UsersController : Controller
    {
        public readonly TextPortDA da = new TextPortDA();
        public readonly TextPortContext _context = new TextPortContext();

        [Authorize]
        public ActionResult Index()
        {
            UserListContainer UsersContainer = new UserListContainer();
            return View(UsersContainer);
        }

        [Authorize]
        [HttpGet]
        [ActionName("add-user")]
        public ActionResult Add()
        {
            UserViewModel uvm = new UserViewModel();
            return PartialView("_AddUser", uvm);
        }

        [Authorize]
        [HttpPost]
        [ActionName("add-user")]
        public ActionResult Add(UserViewModel uvm)
        {
            try
            {
                int NewUserId = da.AddAccount(uvm);

                return View("Index", new UserListContainer());
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
            }
            return null;
        }

        [Authorize]
        [HttpGet]
        [ActionName("edit-user")]
        public ActionResult Edit(int id)
        {
            UserViewModel bvm = new UserViewModel(id);
            return PartialView("_EditUser", bvm);
        }

        [Authorize]
        [HttpPost]
        [ActionName("edit-user")]
        public ActionResult Edit(UserViewModel uvm)
        {
            try
            {
                da.UpdateAccount(uvm);

                if (!string.IsNullOrEmpty(uvm.Password) && !string.IsNullOrEmpty(uvm.ConfirmPassword))
                {
                    if (uvm.Password.Equals(uvm.ConfirmPassword))
                    {
                        Account acc = _context.Accounts.FirstOrDefault(x => x.AccountId == uvm.AccountId);
                        if (acc != null)
                        {
                            try
                            {
                                acc.Password = AESEncryptDecrypt.Encrypt(uvm.Password, TextPortCore.Helpers.Constants.RC4Key);
                                int changes = _context.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                string bar = ex.Message;
                            }
                        }
                    }
                }

                return View("Index", new UserListContainer());
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
            }
            return null;
        }

        [Authorize]
        [HttpPost]
        [ActionName("delete-User")]
        public ActionResult Delete(UserViewModel bvm)
        {
            try
            {
                //da.DeleteAccount(bvm);

                return View("Index", new UserListContainer());
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
            }
            return null;
        }

        [HttpPost]
        [ActionName("list-search")]
        public ActionResult DropdownSearch(UserSearchParams searchParams)
        {
            UserSearchResults results = PerformUserSearch(searchParams);
            return Json(results);
        }

        [HttpPost]
        [ActionName("get-User-details")]
        public ActionResult GetUserDetails(int id)
        {
            Account results = _context.Accounts.FirstOrDefault(x => x.AccountId == id);
            return Json(results);
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetUserPage(PagingParameters parameters)
        {
            UserListContainer UsersContainer = new UserListContainer();

            UsersContainer = GetDataFromDatabase(parameters);

            return Json(new
            {
                page = UsersContainer.CurrentPage,
                recordsPerPage = UsersContainer.RecordsPerPage,
                pageCount = UsersContainer.PageCount,
                recordLabel = UsersContainer.RecordLabel,
                sortOrder = UsersContainer.SortOrder,
                html = renderRazorViewToString("_UserList", UsersContainer)
            });
        }

        public UserListContainer GetDataFromDatabase(PagingParameters pp)
        {
            int resultsCount = 0;
            int page = pp.Page;
            int recordsPerPage = pp.RecordsPerPage;
            int previousRecordsPerPage = pp.PreviousRecordsPerPage;
            string sortBy = pp.SortBy;
            string sortOrder = pp.SortOrder;

            string connectionString = ConfigurationManager.ConnectionStrings["TextPortContext"].ConnectionString;
            List<User> results = new List<User>();

            try
            {
                string filter = string.Empty;
                string sqlBase = "SELECT A.AccountId, A.BranchIds, A.UserName, A.Name, A.Phone, A.Email, R.RoleName, B.BranchName FROM Accounts A ";
                sqlBase += "INNER JOIN Roles R ON A.RoleId = R.RoleId INNER JOIN Branches B ON A.BranchId = B.BranchId ";

                // Apply filters
                if (!string.IsNullOrEmpty(pp.SearchString))
                {
                    pp.SearchString = pp.SearchString.ToLower();
                    filter += (!string.IsNullOrEmpty(filter)) ? " AND " : "WHERE ";
                    filter += $" (CHARINDEX('{pp.SearchString}', A.{pp.SearchBy}) > 0) ";
                }

                if (pp.Operation == "sort")
                {
                    if (pp.SortBy == pp.PrevSortBy)
                    {
                        sortOrder = (sortOrder == "asc") ? "desc" : "asc";
                    }
                }

                UserListContainer UserListContainer = new UserListContainer();
                UserListContainer.RecordsPerPage = recordsPerPage;
                UserListContainer.SortOrder = sortOrder;

                // Sorting
                string tablePrefix = "A";
                switch (sortBy)
                {
                    case "BranchName":
                        tablePrefix = "B";
                        break;
                    case "RoleName":
                        tablePrefix = "R";
                        break;
                }
                string sort = $"ORDER BY {tablePrefix}.{sortBy} {sortOrder} ";
                string paging = $"OFFSET {(pp.Page - 1) * recordsPerPage} ROWS FETCH NEXT {recordsPerPage} ROWS ONLY";

                // Build final query
                string query = $"{sqlBase} {filter} {sort} {paging}";

                using (var connection = new SqlConnection(connectionString))
                {
                    resultsCount = connection.ExecuteScalar<int>($"SELECT COUNT(A.AccountId) FROM Accounts A {filter}");

                    results = connection.Query<User>(query).ToList();
                }

                UserListContainer.RecordCount = resultsCount;

                if ((previousRecordsPerPage > 0 && recordsPerPage != previousRecordsPerPage)) // || (prevFilterBy != filterBy))
                {
                    int firstRecord = page * previousRecordsPerPage;
                    if (firstRecord > UserListContainer.RecordCount)
                    {
                        firstRecord = UserListContainer.RecordCount;
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

                UserListContainer.CurrentPage = pp.Page;
                UserListContainer.LowRecord = (pp.RecordsPerPage * (pp.Page - 1)) + 1;
                UserListContainer.HighRecord = UserListContainer.LowRecord + pp.RecordsPerPage - 1;
                UserListContainer.UserList = results;

                if (UserListContainer.HighRecord > UserListContainer.RecordCount)
                {
                    UserListContainer.HighRecord = UserListContainer.RecordCount;
                }

                if (UserListContainer.RecordCount > 0 && UserListContainer.RecordsPerPage > 0)
                {
                    float pageCount = (float)UserListContainer.RecordCount / (float)UserListContainer.RecordsPerPage;
                    UserListContainer.PageCount = (int)pageCount;
                    if (pageCount - UserListContainer.PageCount > 0)
                    {
                        UserListContainer.PageCount++;
                    }
                }

                // Need to return at least 1 as the page count to prevent twbsPagination from erroring.
                if (UserListContainer.PageCount < 1)
                    UserListContainer.PageCount = 1;

                return UserListContainer;
            }
            catch (Exception ex)
            {
                string foo = ex.Message;
            }

            return null;
        }

        public UserSearchResults PerformUserSearch(UserSearchParams sp)
        {
            int recordsPerPage = 50;

            string connectionString = ConfigurationManager.ConnectionStrings["TextPortContext"].ConnectionString;
            UserSearchResults results = new UserSearchResults()
            {
                items = new List<UserSearchResult>()
            };

            List<UserSearchResult> resultItems = new List<UserSearchResult>();

            try
            {
                string filter = string.Empty;
                string sqlBase = "SELECT A.AccountId AS id, A.FirstName + ' ' + A.LastName AS text FROM Accounts A ";

                // Apply filters
                if (!string.IsNullOrEmpty(sp.term))
                {
                    sp.term = sp.term.ToLower();
                    filter += (!string.IsNullOrEmpty(filter)) ? " AND " : "WHERE ";
                    filter += $" (CHARINDEX('{sp.term}', A.FirstName) > 0) ";
                }

                UserSearchResults searchResults = new UserSearchResults();

                // Paging
                string sort = $"ORDER BY A.FirstName ASC ";
                string paging = $"OFFSET {(sp.page - 1) * recordsPerPage} ROWS FETCH NEXT {recordsPerPage} ROWS ONLY";

                // Build final query
                string query = $"{sqlBase} {filter} {sort} {paging}";

                using (var connection = new SqlConnection(connectionString))
                {
                    resultItems = connection.Query<UserSearchResult>(query).ToList();
                }

                results.items = resultItems;

                return results;
            }
            catch (Exception ex)
            {
                string foo = ex.Message;
            }

            return null;
        }

        private string renderRazorViewToString(string viewName, object model)
        {
            try
            {
                ViewData.Model = model;
                using (var sw = new StringWriter())
                {
                    var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                    var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                    viewResult.View.Render(viewContext, sw);
                    viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                    return sw.GetStringBuilder().ToString();
                }
            }
            catch (Exception ex)
            {
                string foo = ex.Message;
                return ex.Message;
            }
        }
    }

    public class UserListParam
    {
        public int UserId { get; set; }
    }

    public class UserSearchParams
    {
        public string term { get; set; }
        public int page { get; set; }
    }

    public class UserSearchResults
    {
        public List<UserSearchResult> items { get; set; }
    }

    public class UserSearchResult
    {
        public string id { get; set; }
        public string text { get; set; }
    }

}