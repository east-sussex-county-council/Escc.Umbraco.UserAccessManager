using System.Collections.Generic;
using UmbracoUserControl.Models;
using UmbracoUserControl.ViewModel;

namespace UmbracoUserControl.Services.Interfaces
{
    public interface IUserControlService
    {
        IList<UmbracoUserModel> LookupUsers(FindUserModel model);

        ContentTreeViewModel LookupUserById(int id);

        bool InitiatePasswordReset(UmbracoUserControl.Models.PasswordResetModel model, string url);

        bool ResetPassword(UmbracoUserControl.Models.PasswordResetModel model);

        bool CreateUser(UmbracoUserControl.Models.UmbracoUserModel model);

        bool ToggleLock(Models.UmbracoUserModel umbracoUserModel);
    }
}