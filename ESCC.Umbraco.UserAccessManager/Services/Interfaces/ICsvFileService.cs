using System.Collections.Generic;
using Escc.Umbraco.UserAccessManager.Models;

namespace Escc.Umbraco.UserAccessManager.Services.Interfaces
{
    interface ICsvFileService
    {
        IList<InspyderLinkModel> GetLinksByDestination(string destinationUrl);
    }
}
