using PagedList;
using System;
using System.Collections.Generic;
using UmbracoUserControl.Models;

namespace UmbracoUserControl.Services
{
    public interface IUserControlService
    {
        IList<UmbracoUserModel> LookupUsers(FindUserModel model);

        bool InitiatePasswordReset(UmbracoUserControl.Models.PasswordResetModel model, string url);

        bool ResetPassword(UmbracoUserControl.Models.PasswordResetModel model);

        bool CreateUser(UmbracoUserControl.Models.UmbracoUserModel model);

        bool ToggleLock(Models.UmbracoUserModel umbracoUserModel);

        IList<ContentTreeModel> GetContentRoot();

        IList<ContentTreeModel> GetContentChild(int id);
    }
}