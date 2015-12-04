using System.Collections.Generic;
using ESCC.Umbraco.UserAccessManager.Models;
using ESCC.Umbraco.UserAccessManager.ViewModel;

namespace ESCC.Umbraco.UserAccessManager.Services.Interfaces
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

        bool SetContentPermissions(PagePermissionsModel model);

        bool RemoveContentPermissions(PagePermissionsModel model);

        IList<PagePermissionsModel> CheckUserPermissions(int userId);

        IList<PagePermissionsModel> CheckPagesWithoutAuthor();

        bool ClonePermissions(PagePermissionsModel model);

        IList<PagePermissionsModel> CheckPagePermissions(string url);

        IList<ExpiringPageModel> GetExpiringPages(int noOfDaysFrom);

        IList<UserPagesModel> GetExpiringPagesByUser(int noOfDaysFrom);

        PageLinksModel FindInboundLinks(string url);
    }
}