using System.Collections.Generic;
using ESCC.Umbraco.UserAccessManager.Models;
using ESCC.Umbraco.UserAccessManager.ViewModel;

namespace ESCC.Umbraco.UserAccessManager.Services.Interfaces
{
    public interface IUserControlService
    {
        IList<UmbracoUserModel> LookupUsers(FindUserModel model);

        ContentTreeViewModel LookupUserById(int id);

        bool InitiatePasswordReset(PasswordResetModel model, string url);

        bool CheckResetDetails(PasswordResetModel model);

        bool ResetPassword(PasswordResetModel model);

        UmbracoUserModel CreateUser(UmbracoUserModel model);

        bool ToggleLock(UmbracoUserModel umbracoUserModel);
    }
}