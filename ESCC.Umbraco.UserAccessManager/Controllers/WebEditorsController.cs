using System.Web.Mvc;
using ESCC.Umbraco.UserAccessManager.Services.Interfaces;

namespace ESCC.Umbraco.UserAccessManager.Controllers
{
    public class WebEditorsController : Controller
    {
        private readonly IUmbracoService _umbracoService;

        public WebEditorsController(IUmbracoService umbracoService)
        {
            _umbracoService = umbracoService;
        }
        // GET: WebEditors
        public ActionResult Index()
        {
            var editorList = _umbracoService.GetWebEditors();

            return PartialView("_WebEditors", editorList);
        }
    }
}