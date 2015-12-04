using System;
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
        public ActionResult PagePermissions(string NodeId)
        {
            var nodeId = Int32.Parse(NodeId);
            var model = new FindPageModel {NodeId = nodeId};
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
                var perms = _permissionsControlService.CheckPagePermissions(url);
                if (perms != null)
                {
                    var authorList = perms.ToList();

                    // Get a list of all other Web Authors
                    var excludeUsers = authorList.Select(x => x.UserId).ToArray();
                    var ex = string.Join(",", excludeUsers);
                    var otherAuthors = _umbracoService.GetWebAuthors(ex);

                    // Combine the two lists. These have PermissionId = 0 to indicate they do not have access
                    foreach (var otherAuthor in otherAuthors)
                    {
                        var p = new PagePermissionsModel
                        {
                            UserId = otherAuthor.UserId,
                            FullName = otherAuthor.FullName,
                            EmailAddress = otherAuthor.EmailAddress,
                            UserLocked = otherAuthor.UserLocked,
                            Username = otherAuthor.UserName,
                            PermissionId = 0
                        };

                        authorList.Add(p);
                    }

                    if (!authorList.IsNullOrEmpty()) return PartialView("PagePermissions/LookupPagePermissions", authorList);
                }

                TempData["Message"] = "This page does not exist.";
                TempData["InputString"] = url;

                return PartialView("ToolsError");
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                TempData["Message"] = string.Format("An error occurred while processing your request.");
                TempData["InputString"] = url;

                return PartialView("ToolsError");
            }
        }

        [AcceptVerbs(HttpVerbs.Get|HttpVerbs.Post)]
        public ActionResult LookupUserPermissions(FindUserModel model)
        {
            try
            {
                var modelList = _permissionsControlService.CheckUserPermissions(model);

                if (!modelList.IsNullOrEmpty()) return PartialView("UserPermissions/LookupUserPermissions", modelList);

                TempData["Message"] = "Either user has no permissions setup or this user does not exist.";
                TempData["InputString"] = model.EmailAddress + model.UserName;

                return PartialView("ToolsError");
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                TempData["Message"] = string.Format("An error occurred while processing your request.");
                TempData["InputString"] = model.EmailAddress + model.UserName;

                return PartialView("ToolsError");
            }
        }

        [HttpGet]
        public ActionResult CheckPagePermissions(string url)
        {
            try
            {
                var modelList = _permissionsControlService.CheckPagePermissions(url);

                if (!modelList.IsNullOrEmpty()) return PartialView("CheckPagePermissions", modelList);

                TempData["Message"] = "Either permissions have not been set for this page or page does not exist.";
                TempData["InputString"] = url;

                return PartialView("ToolsError");
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                TempData["Message"] = string.Format("An error occurred while processing your request.");
                TempData["InputString"] = url;

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
            var model = new FindUserModel {UserName = userName};

            return View("UserPermissions/Index", model);
        }

        [HttpGet]
        public ActionResult CheckUserPermissions(FindUserModel model)
        {
            try
            {
                var modelList = _permissionsControlService.CheckUserPermissions(model);

                if (!modelList.IsNullOrEmpty()) return PartialView("CheckUserPermissions", modelList);

                TempData["Message"] = "Either user has no permissions setup or this user does not exist.";
                TempData["InputString"] = model.EmailAddress + model.UserName;

                return PartialView("ToolsError");
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                TempData["Message"] = string.Format("An error occurred while processing your request.");
                TempData["InputString"] = model.EmailAddress + model.UserName;

                return PartialView("ToolsError");
            }
        }

        [HttpGet]
        public ActionResult CheckPagesWithoutAuthor()
        {
            try
            {
                var modelList = _permissionsControlService.PagesWithoutAuthor();

                if (!modelList.IsNullOrEmpty()) return View("PagesWithoutAuthors", modelList);

                TempData["Message"] = "Unable to find pages without authors.";

                return PartialView("ToolsError");
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                TempData["Message"] = string.Format("An error occurred while processing your request.");

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
                    TempData["Message"] = "The page was not found.";
                    TempData["InputString"] = url;

                    return PartialView("ToolsError");
                }
                if (modelList.PageId == 0)
                {
                    TempData["Message"] = "The page was not found.";
                    TempData["InputString"] = url;

                    return PartialView("ToolsError");
                }

                return PartialView("InboundLinks/CheckInboundLinks", modelList);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                TempData["Message"] = string.Format("An error occurred while processing your request.");
                TempData["InputString"] = url;

                return PartialView("ToolsError");
            }
        }

        [HttpGet]
        public ActionResult TransferToUserPermissions(FindUserModel model)
        {
            try
            {
                var modelList = _permissionsControlService.CheckUserPermissions(model);

                if (!modelList.IsNullOrEmpty())
                {
                    ViewBag.UserPermissionsId = model.UserName;
                    return View("UserPermissions/Index", modelList);
                }

                TempData["Message"] = "Either user has no permissions setup or this user does not exist.";
                TempData["InputString"] = model.EmailAddress + model.UserName;

                return PartialView("ToolsError");
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                TempData["Message"] = string.Format("An error occurred while processing your request.");
                TempData["InputString"] = model.EmailAddress + model.UserName;

                return PartialView("ToolsError");
            }
        }
    }
}