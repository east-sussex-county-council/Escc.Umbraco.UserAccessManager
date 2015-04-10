using System.Collections.Generic;
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

        //bool SyncUserPermissions(int userId);

        bool ClonePermissions(int sourceId, int targetId);

        IEnumerable<PermissionsModel> CheckPagePermissions(string url);

        IList<PermissionsModel> CheckUserPermissions(FindUserModel model);

        IEnumerable<PermissionsModel> PagesWithoutAuthor();

        //void ToggleEditor(ContentTreeViewModel model);
    }
}