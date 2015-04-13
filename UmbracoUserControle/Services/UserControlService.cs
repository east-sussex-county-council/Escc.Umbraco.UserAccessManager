using System;
using System.Collections.Generic;
using Exceptionless;
using UmbracoUserControl.Models;
using UmbracoUserControl.Services.Interfaces;
using UmbracoUserControl.ViewModel;

namespace UmbracoUserControl.Services
{
    public class UserControlService : IUserControlService
    {
        private readonly IUmbracoService _umbracoService;
        private readonly IDatabaseService _databaseService;
        private readonly IEmailService _emailService;

        public UserControlService(IDatabaseService databaseService, IUmbracoService umbracoService, IEmailService emailService)
        {
            _databaseService = databaseService;
            _umbracoService = umbracoService;
            _emailService = emailService;
        }

        public IList<UmbracoUserModel> LookupUsers(FindUserModel model)
        {
            if (!model.IsValidRequest) return null;

            try
            {
                if (model.IsEmailRequest)
                {
                    var modelList = _umbracoService.GetAllUsersByEmail(model.EmailAddress);

                    return modelList;
                }
                if (model.IsUserRequest)
                {
                    var modelList = _umbracoService.GetAllUsersByUsername(model.UserName);

                    return modelList;
                }
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();

                throw;
            }
            return null;
        }

        public ContentTreeViewModel LookupUserById(int id)
        {
            try
            {
                var model = _umbracoService.GetAllUsersById(id);

                //model.isEditor = IsEditor(id);

                return model;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();

                throw;
            }
        }

        /// <summary>
        /// Creates database record and emails user to start reset process
        /// </summary>
        /// <param name="model">PasswordResetModel - EmailAddress and UserId Required</param>
        /// <param name="url">root url for the site eg http://localhost:53201/ </param>
        /// <returns>success bool - all operations complete without error</returns>
        public bool InitiatePasswordReset(PasswordResetModel model, string url)
        {
            model.UniqueResetId = Guid.NewGuid().ToString();

            model.TimeLimit = DateTime.Now;

            try
            {
                _databaseService.SetResetDetails(model);

                _emailService.PasswordResetEmail(model, url);

                return true;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();

                throw;
            }
        }

        /// <summary>
        /// Check Password Reset request is valid
        /// </summary>
        /// <param name="model">Password reset parameters</param>
        /// <returns>True if request is valid</returns>
        public bool CheckResetDetails(PasswordResetModel model)
        {
            var m = _databaseService.GetResetDetails(model);

            // If no record found in the database, then the request is not valid
            if (m == null) return false;

            return DateTime.Now.AddSeconds(-5) <= m.TimeLimit.AddDays(1);
        }

        /// <summary>
        /// Given a valid model, validate agaist database then reset password
        /// </summary>
        /// <param name="model">PasswordResetModel - UniqueResetId, UserId, NewPassword</param>
        /// <returns>success bool - all operations complete without error</returns>
        public bool ResetPassword(PasswordResetModel model)
        {
            try
            {
                var validRequests = _databaseService.GetResetDetails(model);

                // If no record found in the database, then the request is not valid
                if (validRequests == null) return false;

                model.EmailAddress = validRequests.EmailAddress;

                if (DateTime.Now.AddSeconds(-5) <= validRequests.TimeLimit.AddDays(1))
                {
                    _umbracoService.ResetPassword(model);

                    _databaseService.DeleteResetDetails(model);

                    _emailService.PasswordResetConfirmationEmail(model);

                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();

                throw;
            }
            return false;
        }

        /// <summary>
        /// Given a valid model, Create a new user and email the admin team
        /// </summary>
        /// <param name="model">UmbracoUserModel - UserName, FullName, EmailAddress</param>
        /// <returns>success bool - all operations complete without error</returns>
        public bool CreateUser(UmbracoUserModel model)
        {
            try
            {
                _umbracoService.CreateNewUser(model);

                _emailService.CreateNewUserEmail(model);

                return true;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();

                throw;
            }
        }

        /// <summary>
        /// Locks / unlock the users umbraco account
        /// </summary>
        /// <param name="model">UmbracoUserModel - UserId</param>
        /// <returns>success bool - all operations complete without error</returns>
        public bool ToggleLock(UmbracoUserModel model)
        {
            try
            {
                if (model.Lock)
                {
                    _umbracoService.DisableUser(model);
                }
                else
                {
                    _umbracoService.EnableUser(model);
                }
                return true;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();

                throw;
            }
        }

        //private bool IsEditor(int userId)
        //{
        //    try
        //    {
        //        var editors = _databaseService.IsEditor(userId);

        //        return !editors.IsNullOrEmpty();
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToExceptionless().Submit();

        //        throw;
        //    }
        //}
    }
}