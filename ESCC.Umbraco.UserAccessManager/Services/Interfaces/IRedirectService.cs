using System.Collections.Generic;
using Escc.Umbraco.UserAccessManager.Models;

namespace Escc.Umbraco.UserAccessManager.Services.Interfaces
{
    interface IRedirectsService
    {
        IList<RedirectModel> GetRedirectsByDestination(string destinationUrl);
    }
}
