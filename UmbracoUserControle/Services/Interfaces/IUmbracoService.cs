using System;
using System.Collections.Generic;
using UmbracoUserControl.Models;

namespace UmbracoUserControl.Services
{
    public interface IUmbracoService
    {
        void CreateNewUser(UmbracoUserControl.Models.UmbracoUserModel model);

        System.Collections.Generic.IList<UmbracoUserControl.Models.UmbracoUserModel> GetAllUsersByEmail(string emailaddress);

        System.Collections.Generic.IList<UmbracoUserControl.Models.UmbracoUserModel> GetAllUsersByUsername(string username);

        void ResetPassword(UmbracoUserControl.Models.PasswordResetModel model);

        void DisableUser(Models.UmbracoUserModel model);

        void EnableUser(Models.UmbracoUserModel model);

        IList<ContentTreeModel> GetContentRoot();

        IList<ContentTreeModel> GetContentChild(int id);
    }
}