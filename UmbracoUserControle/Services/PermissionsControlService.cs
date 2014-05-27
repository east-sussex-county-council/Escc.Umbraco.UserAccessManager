using Castle.Core.Internal;
using Castle.Core.Logging;
using Microsoft.ApplicationBlocks.ExceptionManagement;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using UmbracoUserControl.Models;
using UmbracoUserControl.Services.Interfaces;
using UmbracoUserControl.ViewModel;

namespace UmbracoUserControl.Services
{
    public class PermissionsControlService : IPermissionsControlService
    {
        private readonly IUmbracoService umbracoService;
        private readonly IDatabaseService databaseService;
        private readonly IUserControlService userControlService;

        private ILogger Logger { get; set; }

        public PermissionsControlService(IDatabaseService databaseService, IUmbracoService umbracoService, IUserControlService userControlService)
        {
            this.databaseService = databaseService;
            this.umbracoService = umbracoService;
            this.userControlService = userControlService;
        }

        private static bool UserHasPermissions(IEnumerable<PermissionsModel> modelList, int pageId)
        {
            return modelList.Any(model => model.PageId == pageId);
        }

        public IList<ContentTreeViewModel> GetContentRoot(ContentTreeViewModel contentModel)
        {
            try
            {
                var modelList = umbracoService.GetContentRoot();
                var pageCheckList = databaseService.CheckUserPermissions(contentModel);
                var permissionsModels = pageCheckList as IList<PermissionsModel> ?? pageCheckList.ToList();

                foreach (var model in modelList)
                {
                    model.key = model.PageId;
                    model.title = model.PageName;
                    model.folder = true;
                    model.lazy = true;
                    model.UserId = contentModel.UserId;

                    if (permissionsModels.IsNullOrEmpty()) continue;

                    if (UserHasPermissions(permissionsModels, model.PageId))
                    {
                        model.selected = true;
                    }
                }

                return modelList;
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("{0} Lookup content tree could not be actioned - error message {1} - Stack trace {2} - inner exception {3}", DateTime.Now, ex.Message, ex.StackTrace, ex.InnerException);

                ExceptionManager.Publish(ex);

                throw;
            }
        }

        public IList<ContentTreeViewModel> GetContentChild(ContentTreeViewModel contentModel)
        {
            try
            {
                var modelList = umbracoService.GetContentChild(contentModel.RootId);
                var pageCheckList = databaseService.CheckUserPermissions(contentModel);
                var permissionsModels = pageCheckList as IList<PermissionsModel> ?? pageCheckList.ToList();

                foreach (var model in modelList)
                {
                    model.key = model.PageId;
                    model.title = model.PageName;
                    model.folder = false;
                    model.lazy = true;
                    model.UserId = contentModel.UserId;

                    if (permissionsModels.IsNullOrEmpty()) continue;

                    if (UserHasPermissions(permissionsModels, model.PageId))
                    {
                        model.selected = true;
                    }
                }

                return modelList;
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("{0} Lookup content tree could not be actioned - error message {1} - Stack trace {2} - inner exception {3}", DateTime.Now, ex.Message, ex.StackTrace, ex.InnerException);

                ExceptionManager.Publish(ex);

                throw;
            }
        }

        public bool SetContentPermissions(ContentTreeViewModel model)
        {
            try
            {
                var user = userControlService.LookupUserById(model.UserId);

                var permissionsModel = new PermissionsModel
                {
                    PageId = model.PageId,
                    UserId = model.UserId,
                    Created = DateTime.Now,
                    FullName = user.FullName,
                    PageName = model.PageName,
                    EmailAddress = user.EmailAddress
                };

                var success = umbracoService.SetContentPermissions(permissionsModel);

                if (!success) return false;

                databaseService.AddUserPermissions(permissionsModel);

                return true;
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("{0} Adding permissionns to database could not be actioned - error message {1} - Stack trace {2} - inner exception {3}", DateTime.Now, ex.Message, ex.StackTrace, ex.InnerException);

                ExceptionManager.Publish(ex);

                throw;
            }
        }

        public bool RemoveContentPermissions(ContentTreeViewModel model)
        {
            try
            {
                var permissionsModel = new PermissionsModel
                {
                    PageId = model.PageId,
                    UserId = model.UserId,
                };

                var success = umbracoService.RemoveContentPermissions(permissionsModel);

                if (!success) return false;

                databaseService.RemoveUserPermissions(permissionsModel);

                return true;
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("{0} Removing permissionns to database could not be actioned - error message {1} - Stack trace {2} - inner exception {3}", DateTime.Now, ex.Message, ex.StackTrace, ex.InnerException);

                ExceptionManager.Publish(ex);

                throw;
            }
        }

        public bool CheckUserPermissions(int userId)
        {
            try
            {
                var permissionsModels = umbracoService.CheckUserPremissions(userId);

                if (permissionsModels.IsNullOrEmpty()) return false;

                databaseService.UpdateUserPermissions(userId, permissionsModels);
                return true;
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("{0} Checking user permissionns could not be actioned - error message {1} - Stack trace {2} - inner exception {3}", DateTime.Now, ex.Message, ex.StackTrace, ex.InnerException);

                ExceptionManager.Publish(ex);

                throw;
            }
        }

        public bool ClonePermissions(int sourceId, int targetId)
        {
            try
            {
                var model = new PermissionsModel { UserId = sourceId, TargetId = targetId };

                var success = umbracoService.ClonePermissions(model);

                if (!success) return false;

                var successDbUpdate = CheckUserPermissions(model.TargetId);

                return successDbUpdate;
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("{0} Cloning user permissionns could not be actioned - error message {1} - Stack trace {2} - inner exception {3}", DateTime.Now, ex.Message, ex.StackTrace, ex.InnerException);

                ExceptionManager.Publish(ex);

                throw;
            }
        }

        public IEnumerable<PermissionsModel> CheckPagePermissions(string url)
        {
            try
            {
                if (!url.Contains("/")) return null;

                var pageName = url.Substring(url.LastIndexOf("/", url.Length) + 1);

                return databaseService.CheckPagePermissions(pageName);
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("{0} Cloning user permissionns could not be actioned - error message {1} - Stack trace {2} - inner exception {3}", DateTime.Now, ex.Message, ex.StackTrace, ex.InnerException);

                ExceptionManager.Publish(ex);

                throw;
            }
        }
    }
}