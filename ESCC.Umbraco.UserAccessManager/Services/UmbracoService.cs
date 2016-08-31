using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using Escc.Umbraco.UserAccessManager.Models;
using Escc.Umbraco.UserAccessManager.Services.Interfaces;
using Escc.Umbraco.UserAccessManager.ViewModel;

namespace Escc.Umbraco.UserAccessManager.Services
{
    public class UmbracoService : IUmbracoService
    {
        private readonly HttpClient _client;

        public UmbracoService()
        {
            var siteUri = ConfigurationManager.AppSettings["SiteUri"];

            siteUri = string.Format("{0}Api/UmbracoUserApi/", siteUri);
            var handler = new HttpClientHandler
            {
                Credentials =
                    new NetworkCredential(ConfigurationManager.AppSettings["apiuser"],
                        ConfigurationManager.AppSettings["apikey"])
            };

            //// Use a proxy server, if one is defined
            //var proxyServer = ConfigurationManager.AppSettings["ProxyServer"];
            //if (!String.IsNullOrEmpty(proxyServer))
            //{
            //    handler.UseDefaultCredentials = true;
            //    handler.Proxy = new WebProxy(proxyServer, false);
            //    handler.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            //    handler.UseProxy = true;
            //}

            // Set a long timeout because some queries have to check all pages and can take a long time
            _client = new HttpClient(handler) { BaseAddress = new Uri(siteUri), Timeout = TimeSpan.FromMinutes(5)};
        }

        /// <summary>
        /// Sends a Get request to a web API with the given URL
        /// </summary>
        /// <param name="uriPath">URL of the web API method</param>
        /// <returns> Response code - Ok or BadGateway</returns>
        private HttpResponseMessage GetMessage(string uriPath)
        {
            var response = _client.GetAsync(uriPath).Result;
            return response;
        }

        /// <summary>
        /// Sends a Post request to a web API wit the given URL
        /// </summary>
        /// <typeparam name="T model">Custom model dependong on method invoked</typeparam>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="uriPath"> of the web API method</param>
        /// <param name="model">request info</param>
        /// <returns>List of items that matches the given model</returns>
        private HttpResponseMessage PostMessage<T>(string uriPath, T model)
        {
            var response = _client.PostAsJsonAsync(uriPath, model).Result;
            return response;
        }

        /// <summary>
        /// Get request to Umbraco web service and retun a list of users with matching emailaddress
        /// </summary>
        /// <param name="emailaddress">Email address to search for</param>
        /// <returns>List of Umbraco User</returns>
        public IList<UmbracoUserModel> GetAllUsersByEmail(string emailaddress)
        {
            var response = GetMessage(string.Format("GetAllUsersByEmail?emailaddress={0}", emailaddress));

            if (!response.IsSuccessStatusCode) return null;
            var modelList = response.Content.ReadAsAsync<IList<UmbracoUserModel>>().Result;
            return modelList;
        }

        public IList<UmbracoUserModel> GetAllUsersByUsername(string username)
        {
            var response = GetMessage(string.Format("GetAllUsersByUsername?username={0}", username));

            if (!response.IsSuccessStatusCode) return null;
            var modelList = response.Content.ReadAsAsync<IList<UmbracoUserModel>>().Result;
            return modelList;
        }

        public IList<UmbracoUserModel> GetWebAuthors(string excludeList)
        {
            var response = GetMessage(string.Format("GetWebAuthors?userIdList={0}", excludeList));

            if (!response.IsSuccessStatusCode) return null;
            var modelList = response.Content.ReadAsAsync<IList<UmbracoUserModel>>().Result;
            return modelList;
        }

        public IList<UmbracoUserModel> GetWebEditors()
        {
            var response = GetMessage("GetWebEditors");

            if (!response.IsSuccessStatusCode) return null;
            var modelList = response.Content.ReadAsAsync<IList<UmbracoUserModel>>().Result;
            return modelList;
        } 

        public ContentTreeViewModel GetAllUsersById(int id)
        {
            var response = GetMessage(string.Format("GetUserById?id={0}", id));

            if (!response.IsSuccessStatusCode) return null;
            var model = response.Content.ReadAsAsync<ContentTreeViewModel>().Result;

            return model;
        }

        /// <summary>
        /// Post request to Umbraco web service to create a new user
        /// </summary>
        /// <param name="model">UmbracoUserModel - UserName, FullName and EmailAddress</param>
        public UmbracoUserModel CreateNewUser(UmbracoUserModel model)
        {
            var response = PostMessage("PostCreateUser", model);

            if (!response.IsSuccessStatusCode) return null;
            model = response.Content.ReadAsAsync<UmbracoUserModel>().Result;

            return model;
        }

        /// <summary>
        /// Post request to Umbraco web service to reset a users password
        /// </summary>
        /// <param name="model">PasswordResetModel - UserId and NewPassword</param>
        public void ResetPassword(PasswordResetModel model)
        {
            var response = PostMessage("PostResetPassword", model);

            if (response.IsSuccessStatusCode) return;
            var ex = response.Content.ReadAsAsync<Exception>().Result;
            throw ex;
        }

        /// <summary>
        /// Post request to umbraco web service to lock the users account
        /// </summary>
        /// <param name="model">UmbracoUserModel - UserId</param>
        public void DisableUser(UmbracoUserModel model)
        {
            var response = PostMessage("PostDisableUser", model);

            if (response.IsSuccessStatusCode) return;
            var ex = response.Content.ReadAsAsync<Exception>().Result;
            throw ex;
        }

        /// <summary>
        /// Post request to umbraco web service to unlock the users account
        /// </summary>
        /// <param name="model">UmbracoUserModel - UserId</param>
        public void EnableUser(UmbracoUserModel model)
        {
            var response = PostMessage("PostEnableUser", model);

            if (response.IsSuccessStatusCode) return;
            var ex = response.Content.ReadAsAsync<Exception>().Result;
            throw ex;
        }

        /// <summary>
        /// Get details from content root node(s)
        /// </summary>
        /// <returns>Details from content root node(s)</returns>
        public IList<ContentTreeViewModel> GetContentRoot()
        {
            var response = GetMessage("GetContentRoot");

            if (!response.IsSuccessStatusCode) return null;
            var modelList = response.Content.ReadAsAsync<IList<ContentTreeViewModel>>().Result;
            return modelList;
        }

        /// <summary>
        /// Get details, including permissions for the supplied user, from content root node(s)
        /// </summary>
        /// <param name="uid">User Id whose permissions should be retrieved</param>
        /// <returns>Details from content root node(s), including permissions for supplied user</returns>
        public IList<ContentTreeViewModel> GetContentRoot(int uid)
        {
            var response = GetMessage(string.Format("GetContentRootUserPerms?userId={0}", uid));

            if (!response.IsSuccessStatusCode) return null;
            var modelList = response.Content.ReadAsAsync<IList<ContentTreeViewModel>>().Result;
            return modelList;
        }

        /// <summary>
        /// Get details from all child nodes of the supplied parent node id.
        /// </summary>
        /// <param name="id">Id of parent node</param>
        /// <returns>List of child nodes</returns>
        public IList<ContentTreeViewModel> GetContentChild(int id)
        {
            var response = GetMessage(string.Format("GetContentChild?id={0}", id));

            if (!response.IsSuccessStatusCode) return null;
            var modelList = response.Content.ReadAsAsync<IList<ContentTreeViewModel>>().Result;
            return modelList;
        }

        /// <summary>
        /// Get details from all child nodes of the supplied parent node id, along with permissions for supplied user.
        /// </summary>
        /// <param name="id">Id of parent node</param>
        /// <param name="uid">User Id</param>
        /// <returns>List of child nodes with permissions for supplied user</returns>
        public IList<ContentTreeViewModel> GetContentChild(int id, int uid)
        {
            var response = GetMessage(string.Format("GetContentChildUserPerms?id={0}&userId={1}", id, uid));

            if (!response.IsSuccessStatusCode) return null;
            var modelList = response.Content.ReadAsAsync<IList<ContentTreeViewModel>>().Result;
            return modelList;
        }

        /// <summary>
        /// Set permissions on content node
        /// </summary>
        /// <param name="model">Details of node, user and permissions</param>
        /// <returns>True if successful</returns>
        public bool SetContentPermissions(PermissionsModel model)
        {
            var response = PostMessage("PostSetPermissions", model);

            return response.IsSuccessStatusCode;
        }

        public bool RemoveContentPermissions(PermissionsModel model)
        {
            var response = PostMessage("PostRemovePermissions", model);

            return response.IsSuccessStatusCode;
        }

        public IList<PermissionsModel> CheckUserPermissions(int userId)
        {
            var response = GetMessage(string.Format("GetCheckUserPermissions?userId={0}", userId));

            if (!response.IsSuccessStatusCode) return null;
            var model = response.Content.ReadAsAsync<IList<PermissionsModel>>().Result;

            return model;
        }

        public PageUsersModel CheckPagePermissions(string url)
        {
            var response = GetMessage(string.Format("GetPagePermissions?url={0}", url));

            if (!response.IsSuccessStatusCode) return null;
            var modelList = response.Content.ReadAsAsync<PageUsersModel>().Result;
            return modelList;
        }

        public IList<PermissionsModel> CheckPagesWithoutAuthor()
        {
            var response = GetMessage("GetPagesWithoutAuthor");

            if (!response.IsSuccessStatusCode) return null;
            var modelList = response.Content.ReadAsAsync<IList<PermissionsModel>>().Result;
            return modelList;
        }

        public bool ClonePermissions(PermissionsModel model)
        {
            var response = PostMessage("PostCloneUserPermissions", model);

            return response.IsSuccessStatusCode;
        }

        public IList<ExpiringPageModel> GetExpiringPages(int noOfDaysFrom)
        {
            var response = GetMessage(string.Format("CheckForExpiringNodes?noofdaysfrom={0}", noOfDaysFrom));

            if (!response.IsSuccessStatusCode) return null;
            var modelList = response.Content.ReadAsAsync<IList<ExpiringPageModel>>().Result;
            return modelList;
        }

        public IList<UserPagesModel> GetExpiringPagesByUser(int noOfDaysFrom)
        {
            var response = GetMessage(string.Format("CheckForExpiringNodesByUser?noofdaysfrom={0}", noOfDaysFrom));

            if (!response.IsSuccessStatusCode) return null;
            var modelList = response.Content.ReadAsAsync<IList<UserPagesModel>>().Result;
            return modelList;
        }
    }
}