using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UmbracoUserControl.Models;
using UmbracoUserControl.ViewModel;

namespace UmbracoUserControl.Services.Interfaces
{
    public interface IPermissionsControlService
    {
        IList<ContentTreeViewModel> GetContentRoot(ContentTreeViewModel userId);

        IList<ContentTreeViewModel> GetContentChild(ContentTreeViewModel userId);

        bool SetContentPermissions(ContentTreeViewModel model);

        bool RemoveContentPermissions(ContentTreeViewModel model);

        bool CheckUserPermissions(int userId);

        bool ClonePermissions(int sourceId, int targetId);

        IEnumerable<PermissionsModel> CheckPagePermissions(string url);
    }
}