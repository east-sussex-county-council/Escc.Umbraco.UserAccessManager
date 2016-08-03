using System.Collections.Generic;
using Escc.Umbraco.UserAccessManager.Models;
using Escc.Umbraco.UserAccessManager.ViewModel;

namespace Escc.Umbraco.UserAccessManager.Services.Interfaces
{
    public interface IUmbracoService
    {
        UmbracoUserModel CreateNewUser(UmbracoUserModel model);

        IList<UmbracoUserModel> GetAllUsersByEmail(string emailaddress);

        IList<UmbracoUserModel> GetAllUsersByUsername(string username);

        ContentTreeViewModel GetAllUsersById(int id);

        IList<UmbracoUserModel> GetWebAuthors(string excludeList);

        IList<UmbracoUserModel> GetWebEditors();

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

        PageUsersModel CheckPagePermissions(string url);

        IList<ExpiringPageModel> GetExpiringPages(int noOfDaysFrom);

        IList<UserPagesModel> GetExpiringPagesByUser(int noOfDaysFrom);
    }
}