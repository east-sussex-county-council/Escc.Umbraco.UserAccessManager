using System.Collections.Generic;
using ESCC.Umbraco.UserAccessManager.Models;
using ESCC.Umbraco.UserAccessManager.ViewModel;

namespace ESCC.Umbraco.UserAccessManager.Services.Interfaces
{
    public interface IUmbracoService
    {
        void CreateNewUser(UmbracoUserModel model);

        IList<UmbracoUserModel> GetAllUsersByEmail(string emailaddress);

        IList<UmbracoUserModel> GetAllUsersByUsername(string username);

        ContentTreeViewModel GetAllUsersById(int id);

        void ResetPassword(PasswordResetModel model);

        void DisableUser(UmbracoUserModel model);

        void EnableUser(UmbracoUserModel model);

        IList<ContentTreeViewModel> GetContentRoot();

        IList<ContentTreeViewModel> GetContentRoot(int uid);

        IList<ContentTreeViewModel> GetContentChild(int id);

        IList<ContentTreeViewModel> GetContentChild(int id, int uid);

        bool SetContentPermissions(PermissionsModel model);

        bool RemoveContentPermissions(PermissionsModel model);

        IList<PermissionsModel> CheckUserPermissions(int userId);

        IList<PermissionsModel> CheckPagesWithoutAuthor();

        bool ClonePermissions(PermissionsModel model);

        IList<PermissionsModel> CheckPagePermissions(string url);

        IList<ExpiringPageModel> GetExpiringPages(int noOfDaysFrom);
    }
}