using Castle.Core.Internal;
using Castle.Core.Logging;
using Microsoft.ApplicationBlocks.ExceptionManagement;
using System;
using System.Collections.Generic;
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
        private readonly IEmailService emailService;

        private ILogger Logger { get; set; }

        public PermissionsControlService(IDatabaseService databaseService, IUmbracoService umbracoService, IEmailService emailService)
        {
            this.databaseService = databaseService;
            this.umbracoService = umbracoService;
            this.emailService = emailService;
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

                foreach (var model in modelList)
                {
                    model.key = model.PageId;
                    model.title = model.PageName;
                    model.folder = true;
                    model.lazy = true;
                    model.selected = true;
                    model.UserId = contentModel.UserId;

                    var pageCheckList = databaseService.CheckUserPermissions(contentModel);

                    if (pageCheckList.IsNullOrEmpty())
                    {
                        if (UserHasPermissions(pageCheckList, model.PageId))
                        {
                            model.selected = true;
                        }
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

                foreach (var model in modelList)
                {
                    model.key = model.PageId;
                    model.title = model.PageName;
                    model.folder = false;
                    model.lazy = true;
                    model.UserId = contentModel.UserId;

                    var pageCheckList = databaseService.CheckUserPermissions(contentModel);

                    if (pageCheckList.IsNullOrEmpty())
                    {
                        if (UserHasPermissions(pageCheckList, model.PageId))
                        {
                            model.selected = true;
                        }
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
            throw new NotImplementedException();
        }
    }
}