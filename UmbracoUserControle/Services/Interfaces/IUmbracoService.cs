using System;

namespace UmbracoUserControl.Services
{
    public interface IUmbracoService
    {
        void CreateNewUser(UmbracoUserControl.Models.UmbracoUserModel model);

        System.Collections.Generic.IList<UmbracoUserControl.Models.UmbracoUserModel> GetAllUsersByEmail(string emailaddress);

        void ResetPassword(UmbracoUserControl.Models.PasswordResetModel model);

        void DisableUser(Models.UmbracoUserModel model);

        void EnableUser(Models.UmbracoUserModel model);
    }
}