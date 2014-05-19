using System.Collections.Generic;
using UmbracoUserControl.Models;
using UmbracoUserControl.ViewModel;

namespace UmbracoUserControl.Services.Interfaces
{
    public interface IUmbracoService
    {
        void CreateNewUser(UmbracoUserControl.Models.UmbracoUserModel model);

        System.Collections.Generic.IList<UmbracoUserControl.Models.UmbracoUserModel> GetAllUsersByEmail(string emailaddress);

        System.Collections.Generic.IList<UmbracoUserControl.Models.UmbracoUserModel> GetAllUsersByUsername(string username);

        ContentTreeViewModel GetAllUsersById(int id);

        void ResetPassword(UmbracoUserControl.Models.PasswordResetModel model);

        void DisableUser(Models.UmbracoUserModel model);

        void EnableUser(Models.UmbracoUserModel model);

        IList<ContentTreeViewModel> GetContentRoot();

        IList<ContentTreeViewModel> GetContentChild(int id);

        bool SetContentPermissions(PermissionsModel model);

        bool RemoveContentPermissions(PermissionsModel model);

        IList<PermissionsModel> CheckUserPremissions(int userId);

        bool ClonePermissions(PermissionsModel model);
    }
}