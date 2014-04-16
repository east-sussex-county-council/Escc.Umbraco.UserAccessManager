using System;

namespace UmbracoUserControl.Services
{
    public interface IUserControlService
    {
        bool InitiatePasswordReset(UmbracoUserControl.Models.PasswordResetModel model, string url);

        bool ResetPassword(UmbracoUserControl.Models.PasswordResetModel model);

        bool CreateUser(UmbracoUserControl.Models.UmbracoUserModel model);

        bool ToggleLock(Models.UmbracoUserModel umbracoUserModel);
    }
}