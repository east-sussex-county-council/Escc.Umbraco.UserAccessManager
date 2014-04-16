using System;

namespace UmbracoUserControl.Services
{
    public interface IDatabaseService
    {
        void DeleteResetDetails(UmbracoUserControl.Models.PasswordResetModel model);

        UmbracoUserControl.Models.PasswordResetModel GetResetDetails(UmbracoUserControl.Models.PasswordResetModel model);

        void SetResetDetails(UmbracoUserControl.Models.PasswordResetModel model);
    }
}