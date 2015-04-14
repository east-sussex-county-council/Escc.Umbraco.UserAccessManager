using System.Collections.Generic;
using ESCC.Umbraco.UserAccessManager.Models;
using ESCC.Umbraco.UserAccessManager.ViewModel;

namespace ESCC.Umbraco.UserAccessManager.Services.Interfaces
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