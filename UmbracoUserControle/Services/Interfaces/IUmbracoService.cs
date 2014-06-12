using System.Collections.Generic;
using UmbracoUserControl.Models;
using UmbracoUserControl.ViewModel;

namespace UmbracoUserControl.Services.Interfaces
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

        IList<ContentTreeViewModel> GetContentChild(int id);

        bool SetContentPermissions(PermissionsModel model);

        bool RemoveContentPermissions(PermissionsModel model);

        IList<PermissionsModel> CheckUserPremissions(int userId);

        bool ClonePermissions(PermissionsModel model);
    }
}