using System.Collections.Generic;
using Escc.Umbraco.UserAccessManager.Models;
using Escc.Umbraco.UserAccessManager.ViewModel;

namespace Escc.Umbraco.UserAccessManager.Services.Interfaces
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