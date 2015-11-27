using System.Collections.Generic;
using ESCC.Umbraco.UserAccessManager.Models;

namespace ESCC.Umbraco.UserAccessManager.Services.Interfaces
{
    interface ICsvFileService
    {
        IList<InspyderLinkModel> GetLinksByDestination(string destinationUrl);
    }
}
