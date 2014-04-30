using Castle.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UmbracoUserControl.Models;
using UmbracoUserControl.Services;

namespace UmbracoUserControl.Controllers
{
    public class PermissionsController : Controller
    {
        private IUserControlService userControlService;

        //private IUmbracoService umbracoService;
        private ILogger Logger { get; set; }

        public PermissionsController(IUserControlService userControlService)
        {
            this.userControlService = userControlService;
        }

        // GET: Permissions
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LookupContentTree(IList<ContentTreeModel> modelList)
        {
            //modelList = userControlService.GetContentRoot();
            //var id = modelList.First().Id;
            //modelList.Add(userControlService.GetContentChild(id));

            return PartialView("ContentTree", modelList);
        }
    }
}