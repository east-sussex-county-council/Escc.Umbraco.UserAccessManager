using Castle.Components.DictionaryAdapter.Xml;
using Castle.Core.Logging;
using Microsoft.Ajax.Utilities;
using Microsoft.ApplicationBlocks.ExceptionManagement;
using PagedList;
using System;
using System.Collections.Generic;
using UmbracoUserControl.Models;
using UmbracoUserControl.Services.Interfaces;
using UmbracoUserControl.ViewModel;

namespace UmbracoUserControl.Services
{
    public class UserControlService : IUserControlService
    {
        private readonly IUmbracoService umbracoService;
        private readonly IDatabaseService databaseService;
        private readonly IEmailService emailService;

        private ILogger Logger { get; set; }

        public UserControlService(IDatabaseService databaseService, IUmbracoService umbracoService, IEmailService emailService)
        {
            this.databaseService = databaseService;
            this.umbracoService = umbracoService;
            this.emailService = emailService;
        }

        public IList<UmbracoUserModel> LookupUsers(FindUserModel model)
        {
            if (!model.IsValidRequest) return null;

            try
            {
                if (model.IsEmailRequest)
                {
                    var modelList = umbracoService.GetAllUsersByEmail(model.EmailAddress);

                    return modelList;
                }
                if (model.IsUserRequest)
                {
                    var modelList = umbracoService.GetAllUsersByUsername(model.UserName);

                    return modelList;
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("{0} User lookup could not be actioned - error message {1} - Stack trace {2} - inner exception {3}", DateTime.Now, ex.Message, ex.StackTrace, ex.InnerException);

                ExceptionManager.Publish(ex);

                throw;
            }
            return null;
        }

        public ContentTreeViewModel LookupUserById(int id)
        {
            try
            {
                var model = umbracoService.GetAllUsersById(id);

                return model;
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("{0} User lookup could not be actioned - error message {1} - Stack trace {2} - inner exception {3}", DateTime.Now, ex.Message, ex.StackTrace, ex.InnerException);
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
                databaseService.SetResetDetails(model);

                emailService.PasswordResetEmail(model, url);

                return true;
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("{0} Password reset {1}, {2} could not be initiated - error message {3} - Stack trace {4} - inner exception {5}", DateTime.Now, model.UniqueResetId, model.UserId, ex.Message, ex.StackTrace, ex.InnerException);

                ExceptionManager.Publish(ex);

                throw;
            }
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
                var validRequests = databaseService.GetResetDetails(model);

                model.EmailAddress = validRequests.EmailAddress;

                if (DateTime.Now.AddSeconds(-5) <= validRequests.TimeLimit.AddDays(1))
                {
                    umbracoService.ResetPassword(model);

                    databaseService.DeleteResetDetails(model);

                    emailService.PasswordResetConfirmationEmail(model);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("{0} Password reset {1}, {2} could not be actioned - error message {3} - Stack trace {4} - inner exception {5}", DateTime.Now, model.UniqueResetId, model.UserId, ex.Message, ex.StackTrace, ex.InnerException);

                ExceptionManager.Publish(ex);

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
                umbracoService.CreateNewUser(model);

                emailService.CreateNewUserEmail(model);

                return true;
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("{0} User creation {1}, {2} could not be actioned - error message {3} - Stack trace {4} - inner exception {5}", DateTime.Now, model.UserName, model.UserId, ex.Message, ex.StackTrace, ex.InnerException);

                ExceptionManager.Publish(ex);

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
                if (model.Lock != false)
                {
                    umbracoService.DisableUser(model);
                }
                else
                {
                    umbracoService.EnableUser(model);
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("{0} lock / unlock on user account {1}, {2} could not be actioned - error message {3} - Stack trace {4} - inner exception {5}", DateTime.Now, model.UserName, model.UserId, ex.Message, ex.StackTrace, ex.InnerException);

                ExceptionManager.Publish(ex);

                throw;
            }
        }
    }
}