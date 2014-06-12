using Castle.Core.Internal;
using Castle.Core.Logging;
using Exceptionless;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
                var pageCheckList = databaseService.CheckUserPermissions(contentModel.UserId);
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
                ex.ToExceptionless().Submit();

                throw;
            }
        }

        public IList<ContentTreeViewModel> GetContentChild(ContentTreeViewModel contentModel)
        {
            try
            {
                var modelList = umbracoService.GetContentChild(contentModel.RootId);
                var pageCheckList = databaseService.CheckUserPermissions(contentModel.UserId);
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
                ex.ToExceptionless().Submit();

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
                ex.ToExceptionless().Submit();

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
                ex.ToExceptionless().Submit();

                throw;
            }
        }

        public bool SyncUserPermissions(int userId)
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
                ex.ToExceptionless().Submit();

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

                var successDbUpdate = SyncUserPermissions(model.TargetId);

                return successDbUpdate;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();

                throw;
            }
        }

        public IEnumerable<PermissionsModel> CheckPagePermissions(string url)
        {
            try
            {
                if (url.IsNullOrWhiteSpace()) return null;

                var page = new Uri(url).AbsolutePath;

                var pageName = page.Trim('/');

                if (pageName.IsNullOrWhiteSpace())
                {
                    pageName = ConfigurationManager.AppSettings["HomePage"];
                }

                var modelList = databaseService.CheckPagePermissions(pageName);

                var permissionsModels = modelList as IList<PermissionsModel> ?? modelList.ToList();

                return permissionsModels;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();

                throw;
            }
        }

        public IList<PermissionsModel> CheckUserPermissions(FindUserModel model)
        {
            try
            {
                var user = userControlService.LookupUsers(model);

                if (user.IsNullOrEmpty()) return null;

                var modelList = databaseService.CheckUserPermissions(user.First().UserId);

                var permissionsModels = modelList as IList<PermissionsModel> ?? modelList.ToList();

                return permissionsModels;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();

                throw;
            }
        }

        public IEnumerable<PermissionsModel> PagesWithoutAuthor()
        {
            try
            {
                var permissionList = databaseService.PageWithoutAuthor();

                var pagesWithoutAuthor = permissionList as PermissionsModel[] ?? permissionList.ToArray();

                return !pagesWithoutAuthor.IsNullOrEmpty() ? pagesWithoutAuthor : null;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();

                throw;
            }
        }

        public void ToggleEditor(ContentTreeViewModel model)
        {
            try
            {
                var editermodel = new EditorModel { UserId = model.UserId, FullName = model.FullName };
                if (model.isEditor)
                {
                    databaseService.DeleteEditor(editermodel);
                    return;
                }
                databaseService.SetEditor(editermodel);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();

                throw;
            }
        }
    }
}