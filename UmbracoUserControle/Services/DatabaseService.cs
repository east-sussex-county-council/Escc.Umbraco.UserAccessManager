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
        private readonly PetaPoco.Database db;

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
            return db.Query<PasswordResetModel>("SELECT [TimeLimit],[EmailAddress] FROM passwordReset WHERE [ResetId] = @0 and [UserId] = @1", model.UniqueResetId, model.UserId).FirstOrDefault();
        }

        /// <summary>
        /// Run SP to Delete the reset record after reset has been complete, to avoid duplication
        /// </summary>
        /// <param name="model">PasswordResetModel - UniqueResetId and UserId</param>
        public void DeleteResetDetails(PasswordResetModel model)
        {
            db.Execute("Exec DeleteResetDetails @UniqueResetId, @UserId", new { model.UniqueResetId, model.UserId });
        }

        public IEnumerable<PermissionsModel> CheckUserPermissions(int userId)
        {
            return db.Query<PermissionsModel>("SELECT * FROM [UmbracoUserAdminTest].[dbo].[permissions] where [UserId] = @0", userId);
        }

        public void AddUserPermissions(PermissionsModel model)
        {
            db.Insert(model);
        }

        public void RemoveUserPermissions(PermissionsModel model)
        {
            db.Delete<PermissionsModel>("WHERE PageId = @0 and UserId = @1", model.PageId, model.UserId);
        }

        public void UpdateUserPermissions(int userId, IList<PermissionsModel> permissionsModelList)
        {
            using (var transaction = db.GetTransaction())
            {
                db.Delete<PermissionsModel>("WHERE UserId = @0", userId);

                foreach (var permission in permissionsModelList)
                {
                    permission.Created = DateTime.Now;

                    db.Insert(permission);
                }

                transaction.Complete();
            }
        }

        public IEnumerable<PermissionsModel> CheckPagePermissions(string pageName)
        {
            return db.Query<PermissionsModel>("Where PageName = @0", pageName);
        }
    }
}