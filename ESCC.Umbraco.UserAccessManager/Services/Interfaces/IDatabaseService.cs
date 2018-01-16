using Escc.Umbraco.UserAccessManager.Models;
using System.Collections.Generic;

namespace Escc.Umbraco.UserAccessManager.Services.Interfaces
{
    public interface IDatabaseService
    {
        void DeleteResetDetails(PasswordResetModel model);
        PasswordResetModel GetResetDetails(PasswordResetModel model);
        void SetResetDetails(PasswordResetModel model);
    }
}