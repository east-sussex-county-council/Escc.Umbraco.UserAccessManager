using ESCC.Umbraco.UserAccessManager.Models;

namespace ESCC.Umbraco.UserAccessManager.Services.Interfaces
{
    public interface IEmailService
    {
        void PasswordResetConfirmationEmail(PasswordResetModel model);

        void PasswordResetEmail(PasswordResetModel model, string url);

        void CreateNewUserEmail(UmbracoUserModel model);

        void PageExpiryWarningEmail(string emailTo, ExpiringPageModel contentNode, UmbracoUserModel pageUser);
        
        void UserPageExpiryEmail(string emailTo, UserPagesModel userPages);
    }
}