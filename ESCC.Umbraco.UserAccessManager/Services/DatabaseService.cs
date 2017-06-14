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

        #region PageExpiryStats
        /// <summary>
        /// Run SP to input the expiry email details into the database
        /// </summary>
        /// <param name="model">ExpiryLogModel - </param>
        public void SetExpiryLogDetails(ExpiryLogModel model)
        {
            _db.Execute("EXEC SetExpiryLogDetails @EmailAddress, @DateAdded, @EmailSuccess, @Pages", new { model.EmailAddress, model.DateAdded, model.EmailSuccess, model.Pages });
        }

        /// <summary>
        /// Run query to get all expiry logs
        /// </summary>
        /// <param name="model">ExpiryLogModel - </param>
        public List<ExpiryLogModel> GetExpiryLogs()
        {
            List<ExpiryLogModel> expiryEmails = new List<ExpiryLogModel>();
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                cn.Open();
                var sql = string.Format("SELECT * FROM ExpiryEmails");
                SqlCommand sqlCommand = new SqlCommand(sql, cn);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    ExpiryLogModel model = new ExpiryLogModel(0, null, DateTime.Now, false, null);
                    model.Pages = reader["Pages"].ToString();
                    model.DateAdded = (DateTime)reader["DateAdded"];
                    model.EmailAddress = reader["EmailAddress"].ToString();
                    model.EmailSuccess = (bool)reader["EmailSuccess"];
                    model.ID = (int)reader["ID"];
                    expiryEmails.Add(model);
                }
                cn.Close();
            }

            return expiryEmails;
        }

        /// <summary>
        /// Run query to get successful expiry logs
        /// </summary>
        /// <param name="model">ExpiryLogModel - </param>
        public List<ExpiryLogModel> GetExpiryLogSuccessDetails()
        {
            List<ExpiryLogModel> expiryEmails = new List<ExpiryLogModel>();
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                cn.Open();
                var sql = string.Format("SELECT * FROM ExpiryEmails WHERE [EmailSuccess] = {0}", 1);
                SqlCommand sqlCommand = new SqlCommand(sql, cn);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    ExpiryLogModel model = new ExpiryLogModel(0, null, DateTime.Now, false, null);
                    model.Pages = reader["Pages"].ToString();
                    model.DateAdded = (DateTime)reader["DateAdded"];
                    model.EmailAddress = reader["EmailAddress"].ToString();
                    model.EmailSuccess = (bool)reader["EmailSuccess"];
                    model.ID = (int)reader["ID"];
                    expiryEmails.Add(model);
                }
                cn.Close();
            }

            return expiryEmails;
        }

        /// <summary>
        /// Run query to get failed expiry logs
        /// </summary>
        /// <param name="model">ExpiryLogModel - </param>
        public List<ExpiryLogModel> GetExpiryLogFailureDetails()
        {
            List<ExpiryLogModel> expiryEmails = new List<ExpiryLogModel>();
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                cn.Open();
                var sql = string.Format("SELECT * FROM ExpiryEmails WHERE [EmailSuccess] = {0}", 0);
                SqlCommand sqlCommand = new SqlCommand(sql, cn);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    ExpiryLogModel model = new ExpiryLogModel(0, null, DateTime.Now, false, null);
                    model.Pages = reader["Pages"].ToString();
                    model.DateAdded = (DateTime)reader["DateAdded"];
                    model.EmailAddress = reader["EmailAddress"].ToString();
                    model.EmailSuccess = (bool)reader["EmailSuccess"];
                    model.ID = (int)reader["ID"];
                    expiryEmails.Add(model);
                }
                cn.Close();
            }

            return expiryEmails;
        }

        /// <summary>
        /// Run query to get a single log entry
        /// </summary>
        /// <param name="model">ExpiryLogModel - </param>
        public ExpiryLogModel GetExpiryLogByID(int id)
        {
            ExpiryLogModel expiryEmail = new ExpiryLogModel();
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                cn.Open();
                var sql = string.Format("SELECT * FROM ExpiryEmails WHERE [ID] = {0}", id);
                SqlCommand sqlCommand = new SqlCommand(sql, cn);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    ExpiryLogModel model = new ExpiryLogModel(0, null, DateTime.Now, false, null);
                    model.Pages = reader["Pages"].ToString();
                    model.DateAdded = (DateTime)reader["DateAdded"];
                    model.EmailAddress = reader["EmailAddress"].ToString();
                    model.EmailSuccess = (bool)reader["EmailSuccess"];
                    model.ID = (int)reader["ID"];
                    expiryEmail = model;
                }
                cn.Close();
            }

            return expiryEmail;
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

        #region Misc Unused
        //public IEnumerable<PermissionsModel> CheckUserPermissions(int userId)
        //{
        //    return _db.Query<PermissionsModel>("SELECT * FROM permissions where [UserId] = @0", userId);
        //}

        //public void AddUserPermissions(PermissionsModel model)
        //{
        //    _db.Insert(model);
        //}

        //public void RemoveUserPermissions(PermissionsModel model)
        //{
        //    _db.Delete<PermissionsModel>("WHERE PageId = @0 and UserId = @1", model.PageId, model.UserId);
        //}

        //public void UpdateUserPermissions(int userId, IList<PermissionsModel> permissionsModelList)
        //{
        //    using (var transaction = _db.GetTransaction())
        //    {
        //        _db.Delete<PermissionsModel>("WHERE UserId = @0", userId);

        //        foreach (var permission in permissionsModelList)
        //        {
        //            permission.Created = DateTime.Now;

        //            _db.Insert(permission);
        //        }

        //        transaction.Complete();
        //    }
        //}

        //public IEnumerable<PermissionsModel> CheckPagePermissions(string pageName)
        //{
        //    return _db.Query<PermissionsModel>("Where PageName = @0", pageName);
        //}
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