using System.Collections.Generic;
using Escc.Umbraco.UserAccessManager.Models;

namespace Escc.Umbraco.UserAccessManager.Services.Interfaces
{
    public interface IEmailService
    {
        void PasswordResetConfirmationEmail(PasswordResetModel model);

        void PasswordResetEmail(PasswordResetModel model, string url);

        void CreateNewUserEmail(UmbracoUserModel model);
        
        void UserPageExpiryEmail(string emailTo, UserPagesModel userPages);

        void UserPageLastWarningEmail(string emailTo, List<UserPageModel> userPages, int emailWebStaffAtDays);
    }
}