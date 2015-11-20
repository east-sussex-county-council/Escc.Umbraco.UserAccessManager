using System.Collections.Generic;
using ESCC.Umbraco.UserAccessManager.Models;

namespace ESCC.Umbraco.UserAccessManager.Services.Interfaces
{
    interface IRedirectsService
    {
        IList<RedirectModel> GetRedirectsByDestination(string destinationUrl);
    }
}
