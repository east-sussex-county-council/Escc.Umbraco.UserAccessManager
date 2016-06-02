using Escc.Umbraco.UserAccessManager.Models;

namespace Escc.Umbraco.UserAccessManager.Services.Interfaces
{
    public interface IDatabaseService
    {
        void DeleteResetDetails(PasswordResetModel model);

        PasswordResetModel GetResetDetails(PasswordResetModel model);

        void SetResetDetails(PasswordResetModel model);

        //IEnumerable<PermissionsModel> CheckUserPermissions(int userId);

        //void AddUserPermissions(PermissionsModel model);

        //void RemoveUserPermissions(PermissionsModel databaseModel);

        //void UpdateUserPermissions(int userId, IList<PermissionsModel> permissionsModelList);

        //IEnumerable<PermissionsModel> CheckPagePermissions(string pageName);

        //IEnumerable<PermissionsModel> PageWithoutAuthor();

        //IEnumerable<EditorModel> IsEditor(int userId);

        //void SetEditor(EditorModel model);

        //void DeleteEditor(EditorModel model);
    }
}