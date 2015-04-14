using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using ESCC.Umbraco.UserAccessManager.Models;
using ESCC.Umbraco.UserAccessManager.Services.Interfaces;
using ESCC.Umbraco.UserAccessManager.ViewModel;

namespace ESCC.Umbraco.UserAccessManager.Services
{
    public class UmbracoService : IUmbracoService
    {
        private readonly HttpClient _client;

        public UmbracoService()
        {
            var siteUri = ConfigurationManager.AppSettings["SiteUri"];

            var handler = new HttpClientHandler
            {
                Credentials =
                    new NetworkCredential(ConfigurationManager.AppSettings["apiuser"],
                        ConfigurationManager.AppSettings["apikey"])
            };
            _client = new HttpClient(handler) { BaseAddress = new Uri(siteUri) };
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
            var response = GetMessage("GetAllUsersByEmail?emailaddress=" + emailaddress);

            if (!response.IsSuccessStatusCode) return null;
            var modelList = response.Content.ReadAsAsync<IList<UmbracoUserModel>>().Result;
            return modelList;
        }

        public IList<UmbracoUserModel> GetAllUsersByUsername(string username)
        {
            var response = GetMessage("GetAllUsersByUsername?username=" + username);

            if (!response.IsSuccessStatusCode) return null;
            var modelList = response.Content.ReadAsAsync<IList<UmbracoUserModel>>().Result;
            return modelList;
        }

        public ContentTreeViewModel GetAllUsersById(int id)
        {
            var response = GetMessage("GetUserById?id=" + id);

            if (!response.IsSuccessStatusCode) return null;
            var model = response.Content.ReadAsAsync<ContentTreeViewModel>().Result;

            return model;
        }

        /// <summary>
        /// Post request to Umbraco web service to create a new user
        /// </summary>
        /// <param name="model">UmbracoUserModel - UserName, FullName and EmailAddress</param>
        public void CreateNewUser(UmbracoUserModel model)
        {
            var response = PostMessage("PostCreateUser", model);

            if (response.IsSuccessStatusCode) return;
            var ex = response.Content.ReadAsAsync<Exception>().Result;
            //var ex = response.Content.ReadAsAsync<PostMessageError>().Result;
            throw ex;
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

        public IList<ContentTreeViewModel> GetContentRoot()
        {
            var response = GetMessage("GetContentRoot");

            if (!response.IsSuccessStatusCode) return null;
            var modelList = response.Content.ReadAsAsync<IList<ContentTreeViewModel>>().Result;
            return modelList;
        }

        public IList<ContentTreeViewModel> GetContentRoot(int uid)
        {
            var response = GetMessage("GetContentRootUserPerms?userId=" + uid);

            if (!response.IsSuccessStatusCode) return null;
            var modelList = response.Content.ReadAsAsync<IList<ContentTreeViewModel>>().Result;
            return modelList;
        }

        public IList<ContentTreeViewModel> GetContentChild(int id)
        {
            var response = GetMessage("GetContentChild?id=" + id);

            if (!response.IsSuccessStatusCode) return null;
            var modelList = response.Content.ReadAsAsync<IList<ContentTreeViewModel>>().Result;
            return modelList;
        }

        public IList<ContentTreeViewModel> GetContentChild(int id, int uid)
        {
            var response = GetMessage("GetContentChildUserPerms?id=" + id + "&userId=" + uid);

            if (!response.IsSuccessStatusCode) return null;
            var modelList = response.Content.ReadAsAsync<IList<ContentTreeViewModel>>().Result;
            return modelList;
        }

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
            var response = GetMessage("GetCheckUserPermissions?userId=" + userId);

            if (!response.IsSuccessStatusCode) return null;
            var model = response.Content.ReadAsAsync<IList<PermissionsModel>>().Result;

            return model;
        }

        public IList<PermissionsModel> CheckPagePermissions(string url)
        {
            var response = GetMessage("GetPagePermissions?url=" + url);

            if (!response.IsSuccessStatusCode) return null;
            var modelList = response.Content.ReadAsAsync<IList<PermissionsModel>>().Result;
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
    }
}