using UmbracoUserControl.Models;

namespace UmbracoUserControl.Services.Interfaces
{
    public interface IEmailService
    {
        void PasswordResetConfirmationEmail(PasswordResetModel model);

        void PasswordResetEmail(PasswordResetModel model, string url);

        void CreateNewUserEmail(UmbracoUserModel model);
    }
}