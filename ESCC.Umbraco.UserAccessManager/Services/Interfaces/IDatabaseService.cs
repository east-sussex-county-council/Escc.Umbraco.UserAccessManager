using Escc.Umbraco.UserAccessManager.Models;
using System.Collections.Generic;

namespace Escc.Umbraco.UserAccessManager.Services.Interfaces
{
    public interface IDatabaseService
    {
        #region PassWordReset
        void DeleteResetDetails(PasswordResetModel model);
        PasswordResetModel GetResetDetails(PasswordResetModel model);
        void SetResetDetails(PasswordResetModel model);
        #endregion

        #region PageExpiryStats
        void SetExpiryLogDetails(ExpiryLogModel model);
        List<ExpiryLogModel> GetExpiryLogs();
        List<ExpiryLogModel> GetExpiryLogSuccessDetails();
        List<ExpiryLogModel> GetExpiryLogFailureDetails();
        ExpiryLogModel GetExpiryLogByID(int id);
        #endregion

        #region Misc
        //IEnumerable<PermissionsModel> CheckUserPermissions(int userId);

        //void AddUserPermissions(PermissionsModel model);

        //void RemoveUserPermissions(PermissionsModel databaseModel);

        //void UpdateUserPermissions(int userId, IList<PermissionsModel> permissionsModelList);

        //IEnumerable<PermissionsModel> CheckPagePermissions(string pageName);

        //IEnumerable<PermissionsModel> PageWithoutAuthor();

        //IEnumerable<EditorModel> IsEditor(int userId);

        //void SetEditor(EditorModel model);

        //void DeleteEditor(EditorModel model);
        #endregion
    }
}