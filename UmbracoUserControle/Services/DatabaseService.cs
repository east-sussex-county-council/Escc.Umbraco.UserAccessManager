using System;
using System.Collections.Generic;
using System.Linq;
using UmbracoUserControl.Models;
using UmbracoUserControl.Services.Interfaces;
using UmbracoUserControl.ViewModel;

namespace UmbracoUserControl.Services
{
    public class DatabaseService : IDatabaseService
    {
        private PetaPoco.Database db;

        public DatabaseService()
        {
            db = new PetaPoco.Database("DefaultConnection");
        }

        /// <summary>
        /// Run SP to input the reset details into the database
        /// </summary>
        /// <param name="model">PasswordResetModel - UniqueResetId, TimeLimit and EmailAddress</param>
        public void SetResetDetails(PasswordResetModel model)
        {
            db.Execute("EXEC SetResetDetails @UniqueResetId, @TimeLimit, @UserId, @EmailAddress", new { model.UniqueResetId, model.TimeLimit, model.UserId, model.EmailAddress });
        }

        /// <summary>
        /// Run query to select the reset record with matching UniqueResetId and UserId
        /// </summary>
        /// <param name="model">PasswordResetModel - UniqueResetId and UserId</param>
        /// <returns>Updated model - TimeLimit and EmailAddress</returns>
        public PasswordResetModel GetResetDetails(PasswordResetModel model)
        {
            foreach (var result in db.Query<PasswordResetModel>("SELECT [TimeLimit],[EmailAddress] FROM passwordReset WHERE [ResetId] = @0 and [UserId] = @1", model.UniqueResetId, model.UserId))
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Run SP to Delete the reset record after reset has been complete, to avoid duplication
        /// </summary>
        /// <param name="model">PasswordResetModel - UniqueResetId and UserId</param>
        public void DeleteResetDetails(PasswordResetModel model)
        {
            db.Execute("Exec DeleteResetDetails @UniqueResetId, @UserId", new { model.UniqueResetId, model.UserId });
        }

        public IEnumerable<PermissionsModel> CheckUserPermissions(ContentTreeViewModel model)
        {
            return db.Query<PermissionsModel>("SELECT * FROM [UmbracoUserAdminTest].[dbo].[permissions] where [UserId] = @0", model.UserId);
        }
    }
}