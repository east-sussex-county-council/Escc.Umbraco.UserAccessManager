using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using ESCC.Umbraco.UserAccessManager.Models;
using ESCC.Umbraco.UserAccessManager.Services.Interfaces;
using ESCC.Umbraco.UserAccessManager.ViewModel;
using Exceptionless;

namespace ESCC.Umbraco.UserAccessManager.Services
{
    public class PermissionsControlService : IPermissionsControlService
    {
        private readonly IUmbracoService _umbracoService;
        private readonly IDatabaseService _databaseService;
        private readonly IUserControlService _userControlService;

        public PermissionsControlService(IDatabaseService databaseService, IUmbracoService umbracoService, IUserControlService userControlService)
        {
            _databaseService = databaseService;
            _umbracoService = umbracoService;
            _userControlService = userControlService;
        }

        public IList<ContentTreeViewModel> GetContentRoot(ContentTreeViewModel contentModel)
        {
            try
            {
                var modelList = _umbracoService.GetContentRoot(contentModel.UserId);
                //var modelList = _umbracoService.GetContentRoot();
                //var pageCheckList = _databaseService.CheckUserPermissions(contentModel.UserId);
                //var permissionsModels = pageCheckList as IList<PermissionsModel> ?? pageCheckList.ToList();

                foreach (var model in modelList)
                {
                    model.key = model.PageId;
                    model.title = model.PageName;
                    model.folder = true;
                    model.lazy = true;
                    model.UserId = contentModel.UserId;

                    // GS Start
                    // if no permissions at all, then there will be only one element which will contain a "-"
                    // If only the default permission then there will only be one element which will contain "F" (Browse Node)
                    if (model.UserPermissions.Count() > 1 || (model.UserPermissions.ElementAt(0)[0] != "-" && model.UserPermissions.ElementAt(0)[0] != "F"))
                    {
                        model.selected = true;
                    }
                    // GS End

                    //if (permissionsModels.IsNullOrEmpty()) continue;

                    //if (UserHasPermissions(permissionsModels, model.PageId))
                    //{
                    //    model.selected = true;
                    //}
                }

                return modelList;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();

                throw;
            }
        }

        //public IList<ContentTreeViewModel> GetChildNodes(ContentTreeViewModel contentModel)
        //{
        //    try
        //    {
        //        var modelList = _umbracoService.GetContentChild(contentModel.PageId);
        //        var pageCheckList = _databaseService.CheckUserPermissions(contentModel.UserId);
        //        var permissionsModels = pageCheckList as IList<PermissionsModel> ?? pageCheckList.ToList();

        //        foreach (var model in modelList)
        //        {
        //            model.key = model.PageId;
        //            model.title = model.PageName;
        //            model.folder = false;
        //            model.lazy = false;
        //            model.UserId = contentModel.UserId;

        //            if (permissionsModels.IsNullOrEmpty()) continue;

        //            if (UserHasPermissions(permissionsModels, model.PageId))
        //            {
        //                model.selected = true;
        //            }
        //        }

        //        return modelList;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToExceptionless().Submit();

        //        throw;
        //    }
        //}

        public IList<ContentTreeViewModel> GetContentChild(ContentTreeViewModel contentModel)
        {
            try
            {
                var modelList = _umbracoService.GetContentChild(contentModel.RootId, contentModel.UserId);
                //var pageCheckList = _databaseService.CheckUserPermissions(contentModel.UserId);
                //var permissionsModels = pageCheckList as IList<PermissionsModel> ?? pageCheckList.ToList();

                foreach (var model in modelList)
                {
                    model.key = model.PageId;
                    model.title = model.PageName;
                    model.folder = false;
                    model.lazy = true;
                    model.UserId = contentModel.UserId;

                    // GS Start
                    // if no permissions at all, then there will be only one element which will contain a "-"
                    // If only the default permission then there will only be one element which will contain "F" (Browse Node)
                    if (model.UserPermissions.Count() > 1 || (model.UserPermissions.ElementAt(0)[0] != "-" && model.UserPermissions.ElementAt(0)[0] != "F"))
                    {
                        model.selected = true;
                    }
                    // GS End

                    //if (permissionsModels.IsNullOrEmpty()) continue;

                    //if (UserHasPermissions(permissionsModels, model.PageId))
                    //{
                    //    model.selected = true;
                    //}
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
                var user = _userControlService.LookupUserById(model.UserId);

                var permissionsModel = new PermissionsModel
                {
                    PageId = model.PageId,
                    UserId = model.UserId,
                    Created = DateTime.Now,
                    FullName = user.FullName,
                    PageName = model.PageName,
                    EmailAddress = user.EmailAddress
                };

                var success = _umbracoService.SetContentPermissions(permissionsModel);

                if (!success) return false;

                //_databaseService.AddUserPermissions(permissionsModel);

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

                var success = _umbracoService.RemoveContentPermissions(permissionsModel);

                if (!success) return false;

                //_databaseService.RemoveUserPermissions(permissionsModel);

                return true;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();

                throw;
            }
        }

        //public bool SyncUserPermissions(int userId)
        //{
        //    try
        //    {
        //        var permissionsModels = _umbracoService.CheckUserPermissions(userId);

        //        //if (permissionsModels.IsNullOrEmpty()) return false;

        //        _databaseService.UpdateUserPermissions(userId, permissionsModels);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToExceptionless().Submit();

        //        throw;
        //    }
        //}

        public bool ClonePermissions(int sourceId, int targetId)
        {
            try
            {
                var model = new PermissionsModel { UserId = sourceId, TargetId = targetId };

                return _umbracoService.ClonePermissions(model);

                //if (!success) return false;

                //var successDbUpdate = SyncUserPermissions(model.TargetId);

                //return successDbUpdate;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();

                throw;
            }
        }

        /// <summary>
        /// Get assigned permissions for a specific page
        /// </summary>
        /// <param name="url">URL of page to check</param>
        /// <returns>List of users with permissions to the supplied page</returns>
        public IEnumerable<PermissionsModel> CheckPagePermissions(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url)) return null;

                //if (!url.Contains("http")) return null;

                //var page = new Uri(url).AbsolutePath;

                //var pageName = page.Trim('/');

                //if (pageName.IsNullOrWhiteSpace())
                //{
                //    pageName = ConfigurationManager.AppSettings["HomePage"];
                //}

                //var modelList = _databaseService.CheckPagePermissions(pageName);

                //var permissionsModels = modelList as IList<PermissionsModel> ?? modelList.ToList();
                var permissionsModels = _umbracoService.CheckPagePermissions(url);

                return permissionsModels;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();

                throw;
            }
        }

        /// <summary>
        /// Get permisions for supplied user
        /// </summary>
        /// <param name="model">user details</param>
        /// <returns>List of pages</returns>
        public IList<PermissionsModel> CheckUserPermissions(FindUserModel model)
        {
            try
            {
                var user = _userControlService.LookupUsers(model);

                if (user.IsNullOrEmpty()) return null;

                //var modelList = _databaseService.CheckUserPermissions(user.First().UserId);

                //var permissionsModels = modelList as IList<PermissionsModel> ?? modelList.ToList();
                var permissionsModels = _umbracoService.CheckUserPermissions(user.First().UserId);

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
                var permissionsModels = _umbracoService.CheckPagesWithoutAuthor();

                return permissionsModels;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();

                throw;
            }
        }

        //public void ToggleEditor(ContentTreeViewModel model)
        //{
        //    try
        //    {
        //        var editermodel = new EditorModel { UserId = model.UserId, FullName = model.FullName };
        //        if (model.isEditor)
        //        {
        //            _databaseService.DeleteEditor(editermodel);
        //            return;
        //        }
        //        _databaseService.SetEditor(editermodel);
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToExceptionless().Submit();

        //        throw;
        //    }
        //}
    }
}