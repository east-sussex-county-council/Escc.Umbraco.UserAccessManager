using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Castle.Core.Internal;
using ESCC.Umbraco.UserAccessManager.Models;
using ESCC.Umbraco.UserAccessManager.Services.Interfaces;
using ESCC.Umbraco.UserAccessManager.Utility;
using Exceptionless;

namespace ESCC.Umbraco.UserAccessManager.Controllers
{
    [AuthorizeRedirect(Roles = SystemRole.WebServices)]
    public class ToolsController : Controller
    {
        private readonly IPermissionsControlService _permissionsControlService;
        private readonly IUmbracoService _umbracoService;

        public ToolsController(IPermissionsControlService permissionsControlService, IUmbracoService umbracoService)
        {
            _permissionsControlService = permissionsControlService;
            _umbracoService = umbracoService;
        }

        // GET: Tools
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult LookupPermissions()
        {
            return View("LookupPermissions/Index");
        }

        [HttpGet]
        public ActionResult PagePermissions()
        {
            return View("PagePermissions/Index");
        }

        [HttpPost]
        public ActionResult PagePermissions(string pageId)
        {
            int pId;
            var pUrl =string.Empty;

            // pageId can be either a numeric page Id or a Url string
            if (!int.TryParse(pageId, out pId))
            {
                pId = int.MinValue;
                pUrl = pageId;
            }

            var model = new FindPageModel {NodeId = pId, Url = pUrl};
            return View("PagePermissions/Index", model);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult LookupPagePermissions(FindPageModel model)
        {
            // Check if this is a Url or NodeId request
            var url = model.IsUrlRequest ? model.Url : model.NodeId.ToString();

            try
            {

                // Get a list of Web Authors that have permission to manage this page
                // Convert to a list so that we can add the other Web Authors
                PageUsersModel perms = _permissionsControlService.CheckPagePermissions(url);
                if (perms != null)
                {
                    List<UserPermissionModel> authorList = perms.Users.ToList();

                    // Get a list of all other Web Authors
                    var excludeUsers = authorList.Select(x => x.UserId).ToArray();
                    var ex = string.Join(",", excludeUsers);
                    var otherAuthors = _umbracoService.GetWebAuthors(ex);

                    // Combine the two lists. These have PermissionId = 0 to indicate they do not have access
                    foreach (var otherAuthor in otherAuthors)
                    {
                        var p = new UserPermissionModel
                        {
                            UserId = otherAuthor.UserId,
                            FullName = otherAuthor.FullName,
                            EmailAddress = otherAuthor.EmailAddress,
                            UserLocked = otherAuthor.UserLocked,
                            UserName = otherAuthor.UserName,
                            PagePermissions = new string[] { }
                        };

                        authorList.Add(p);
                    }

                    perms.Users = authorList;

                    if (!authorList.IsNullOrEmpty()) return PartialView("PagePermissions/LookupPagePermissions", perms);
                }

                TempData["MsgKey"] = "PageNotFound";

                return PartialView("ToolsError");
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                TempData["MsgKey"] = string.Format("ErrorOccurred");

                return PartialView("ToolsError");
            }
        }

        [AcceptVerbs(HttpVerbs.Get|HttpVerbs.Post)]
        public ActionResult LookupUserPermissions(FindUserModel model)
        {
            try
            {
                IList<PermissionsModel> modelList = _permissionsControlService.CheckUserPermissions(model);

                if (!modelList.IsNullOrEmpty()) return PartialView("UserPermissions/LookupUserPermissions", modelList);

                TempData["MsgKey"] = "NoPageOrUser";

                return PartialView("ToolsError");
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                TempData["MsgKey"] = string.Format("ErrorOccurred");

                return PartialView("ToolsError");
            }
        }

        [HttpGet]
        public ActionResult CheckPagePermissions(string url)
        {
            try
            {
                var modelList = _permissionsControlService.CheckPagePermissions(url);

                if (!modelList.Users.IsNullOrEmpty()) return PartialView("CheckPagePermissions", modelList);

                TempData["MsgKey"] = "NoPageOrUser";

                return PartialView("ToolsError");
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                TempData["MsgKey"] = string.Format("ErrorOccurred");

                return PartialView("ToolsError");
            }
        }

        [HttpGet]
        public ActionResult UserPermissions()
        {
            return View("UserPermissions/Index");
        }
        [HttpPost]
        public ActionResult UserPermissions(string userName)
        {
            var model = new PermissionsModel {Username = userName};

            return View("UserPermissions/Index", model);
        }

        [HttpGet]
        public ActionResult CheckUserPermissions(FindUserModel model)
        {
            try
            {
                IList<PermissionsModel> modelList = _permissionsControlService.CheckUserPermissions(model);

                if (!modelList.IsNullOrEmpty()) return PartialView("CheckUserPermissions", modelList);

                TempData["MsgKey"] = "NoPageOrUser";

                return PartialView("ToolsError");
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                TempData["MsgKey"] = string.Format("ErrorOccurred");

                return PartialView("ToolsError");
            }
        }

        [HttpGet]
        public ActionResult CheckPagesWithoutAuthor()
        {
            try
            {
                IEnumerable<PermissionsModel> modelList = _permissionsControlService.PagesWithoutAuthor();

                if (!modelList.IsNullOrEmpty()) return View("PagesWithoutAuthors", modelList);

                TempData["MsgKey"] = "NoAuthorPagesError";

                return PartialView("ToolsError");
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                TempData["MsgKey"] = string.Format("ErrorOccurred");

                return PartialView("ToolsError");
            }
        }

        [HttpGet]
        public ActionResult InboundLinks()
        {
            return View("InboundLinks/Index");
        }

        [HttpGet]
        public ActionResult FindInboundLinks(string url)
        {
            try
            {
                var modelList = _umbracoService.FindInboundLinks(url);

                if (modelList == null)
                {
                    TempData["MsgKey"] = "PageNotFound";

                    return PartialView("ToolsError");
                }
                if (modelList.PageId == 0)
                {
                    TempData["MsgKey"] = "PageNotFound";

                    return PartialView("ToolsError");
                }

                return PartialView("InboundLinks/CheckInboundLinks", modelList);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                TempData["MsgKey"] = string.Format("ErrorOccurred");

                return PartialView("ToolsError");
            }
        }
    }
}