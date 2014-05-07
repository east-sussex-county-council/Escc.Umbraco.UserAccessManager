namespace UmbracoUserControl.Services.Interfaces
{
    public interface IEmailService
    {
        void PasswordResetConfirmationEmail(UmbracoUserControl.Models.PasswordResetModel model);

        void PasswordResetEmail(UmbracoUserControl.Models.PasswordResetModel model, string url);

        void CreateNewUserEmail(UmbracoUserControl.Models.UmbracoUserModel model);
    }
}