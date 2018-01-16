using System.Collections.Generic;
using System.Linq;
using Escc.Umbraco.UserAccessManager.Models;
using Escc.Umbraco.UserAccessManager.Services.Interfaces;
using PetaPoco;
using System.Data.SqlClient;
using System.Configuration;
using System;

namespace Escc.Umbraco.UserAccessManager.Services
{
    public class DatabaseService : IDatabaseService
    {
        #region Initialization
        private readonly Database _db;

        public DatabaseService()
        {
            _db = new Database("DefaultConnection");
        }
        #endregion

        #region PassWord Reset
        /// <summary>
        /// Run SP to input the reset details into the database
        /// </summary>
        /// <param name="model">PasswordResetModel - UniqueResetId, TimeLimit and EmailAddress</param>
        public void SetResetDetails(PasswordResetModel model)
        {
            _db.Execute("EXEC SetResetDetails @UniqueResetId, @TimeLimit, @UserId, @EmailAddress", new { model.UniqueResetId, model.TimeLimit, model.UserId, model.EmailAddress });
        }

        /// <summary>
        /// Run query to select the reset record with matching UniqueResetId and UserId
        /// </summary>
        /// <param name="model">PasswordResetModel - UniqueResetId and UserId</param>
        /// <returns>Updated model - TimeLimit and EmailAddress</returns>
        public PasswordResetModel GetResetDetails(PasswordResetModel model)
        {
            return _db.Query<PasswordResetModel>("SELECT [TimeLimit],[EmailAddress] FROM passwordReset WHERE [ResetId] = @0 and [UserId] = @1", model.UniqueResetId, model.UserId).FirstOrDefault();
        }

        /// <summary>
        /// Run SP to Delete the reset record after reset has been complete, to avoid duplication
        /// </summary>
        /// <param name="model">PasswordResetModel - UniqueResetId and UserId</param>
        public void DeleteResetDetails(PasswordResetModel model)
        {
            _db.Execute("Exec DeleteResetDetails @UniqueResetId, @UserId", new { model.UniqueResetId, model.UserId });
        }
        #endregion

        #region PageAuthor

        public IEnumerable<PermissionsModel> PageWithoutAuthor()
        {
            return _db.Query<PermissionsModel>("SELECT [PageId],[PageName] " +
                                              "FROM permissions as p " +
                                              "left outer join Editors as e on p.userid = e.userid " +
                                              "group by [PageId],[PageName] " +
                                              "having sum(case when e.userid is null then 1 else 0 end) = 0");
        }
        #endregion

        #region Editor
        public IEnumerable<EditorModel> IsEditor(int userId)
        {
            return _db.Query<EditorModel>("Where UserId = @0", userId);
        }

        public void SetEditor(EditorModel model)
        {
            _db.Insert(model);
        }

        public void DeleteEditor(EditorModel model)
        {
            _db.Delete<EditorModel>("WHERE UserId = @0", model.UserId);
        }
        #endregion
    }
}