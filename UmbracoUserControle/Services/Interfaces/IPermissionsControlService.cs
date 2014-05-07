using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UmbracoUserControl.ViewModel;

namespace UmbracoUserControl.Services.Interfaces
{
    public interface IPermissionsControlService
    {
        IList<ContentTreeViewModel> GetContentRoot(ContentTreeViewModel userId);

        IList<ContentTreeViewModel> GetContentChild(ContentTreeViewModel userId);

        bool SetContentPermissions(ContentTreeViewModel model);
    }
}