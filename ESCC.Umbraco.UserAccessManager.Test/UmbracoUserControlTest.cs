using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using ESCC.Umbraco.UserAccessManager.Controllers;
using ESCC.Umbraco.UserAccessManager.Models;
using ESCC.Umbraco.UserAccessManager.Services;
using ESCC.Umbraco.UserAccessManager.Services.Interfaces;
using ESCC.Umbraco.UserAccessManager.ViewModel;
using Moq;
using NUnit.Framework;

namespace UnitTestProject1
{
    [TestFixture]
    public class UmbracoUserControlTest
    {
        private IUserControlService mockUserControlService;
        private IUmbracoService mockUmbracoService;
        private IEmailService mockEmailService;
        private IDatabaseService mockDatabaseService;
        private IPermissionsControlService mockPermissionsControlService;

        private IUserControlService userControlService;
        private IUmbracoService umbracoService;
        private IEmailService emailService;
        private IDatabaseService databaseService;
        private IPermissionsControlService permissionsControlService;

        private DateTime timelimit;
        private PasswordResetModel passWordResetModel;
        private UmbracoUserModel umbracoUserModel;
        private PasswordResetModel passWordResetModelOut;
        private FindUserModel findUserModel;
        private IList<UmbracoUserModel> umbracoUserModelListOut;
        private ContentTreeViewModel contentTreeViewModel;
        private IList<ContentTreeViewModel> contentTreeViewModelListOut;
        private PagePermissionsModel permissionsModel;
        private IList<PagePermissionsModel> permissionsModelsListOut;

        [SetUp]
        public void UmbracoUserControlTestSetup()
        {
            passWordResetModel = new PasswordResetModel();
            passWordResetModelOut = new PasswordResetModel();

            umbracoUserModel = new UmbracoUserModel();
            umbracoUserModelListOut = new List<UmbracoUserModel>();

            findUserModel = new FindUserModel();

            contentTreeViewModel = new ContentTreeViewModel();
            contentTreeViewModelListOut = new List<ContentTreeViewModel>();

            permissionsModel = new PagePermissionsModel();
            permissionsModelsListOut = new List<PagePermissionsModel>();

            mockPermissionsControlService = new Mock<IPermissionsControlService>().Object;
            mockEmailService = new Mock<IEmailService>().Object;

            var userControlServiceMock = new Mock<IUserControlService>();

            userControlServiceMock.Setup(x => x.LookupUserById(2))
                .Returns(contentTreeViewModel);

            mockUserControlService = userControlServiceMock.Object;

            var umbracoServiceMock = new Mock<IUmbracoService>();

            umbracoServiceMock.Setup(x => x.GetAllUsersByEmail("Email"))
                .Returns(umbracoUserModelListOut);
            umbracoServiceMock.Setup(x => x.GetAllUsersByUsername("Username"))
                .Returns(umbracoUserModelListOut);
            umbracoServiceMock.Setup(x => x.GetContentRoot())
                .Returns(contentTreeViewModelListOut);
            umbracoServiceMock.Setup(x => x.GetContentChild(1))
                .Returns(contentTreeViewModelListOut);
            umbracoServiceMock.Setup(x => x.SetContentPermissions(permissionsModel))
                .Returns(true);

            mockUmbracoService = umbracoServiceMock.Object;

            var databaseServiceMock = new Mock<IDatabaseService>();

            databaseServiceMock.Setup(x => x.GetResetDetails(passWordResetModel))
                .Returns(passWordResetModelOut);
            //databaseServiceMock.Setup(x => x.CheckUserPermissions(1))
            //    .Returns(permissionsModelsListOut);

            mockDatabaseService = databaseServiceMock.Object;

            timelimit = DateTime.Now;

            //umbracoService = new UmbracoService();
            //databaseService = new DatabaseService();
            //emailService = new EmailService(mockDatabaseService);
            //userControlService = new UserControlService(mockDatabaseService, mockUmbracoService, mockEmailService);
            //permissionsControlService = new PermissionsControlService(mockDatabaseService, mockUmbracoService, mockUserControlService);
        }

        [Test]
        [TestCase("qwertyui", false)]
        [TestCase("Qwertyui", false)]
        [TestCase("QWERTYUI", false)]
        [TestCase("Q2ERTYUI", false)]
        [TestCase("12345678", false)]
        [TestCase("q2345678", false)]
        [TestCase("Q2345678", false)]
        [TestCase("Q2ertyui", true)]
        [TestCase("TestPass3", true)]
        public void PasswordRest_Regex(string password, bool expected)
        {
            var regex = new Regex(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{8,}$");
            var regexValidated = regex.IsMatch(password);
            Assert.AreEqual(expected, regexValidated, "Password compliance");
        }

        [Test]
        public void AdminController_ResetPassword_RedisplayesView_WhenCalledWithInvalidPasswordModel()
        {
            var controller = new AdminController(mockUserControlService);

            controller.ModelState.AddModelError("error", "errorex");

            var actionResult = controller.ResetPassword(passWordResetModel);

            Assert.IsInstanceOf(typeof(ViewResult), actionResult, "Redisplay view when model state is invalid");

            var viewResult = (ViewResult)actionResult;

            Assert.AreEqual("PasswordResetVerification", viewResult.ViewName);
        }

        [Test]
        public void AdminController_ResetPassword_RedirectToSuccess_WhenCallWithValidPasswordModel()
        {
            passWordResetModel.TimeLimit = DateTime.Now;

            var controller = new AdminController(mockUserControlService);

            var actionResult = controller.ResetPassword(passWordResetModel);

            Assert.IsInstanceOf(typeof(RedirectToRouteResult), actionResult, "Redirect to Success page if reset completes");
        }

        [Test]
        public void AdminController_ResetPassword_RedirectToAction_WhenCalledWithExpiredTimeLimit()
        {
            passWordResetModel.TimeLimit = DateTime.Now.AddDays(-2);

            var controller = new AdminController(mockUserControlService);

            var actionResult = controller.ResetPassword(passWordResetModel);

            Assert.IsInstanceOf(typeof(RedirectToRouteResult), actionResult);
        }

        [Test]
        [TestCase("Test@test.com", 0, true)]
        [TestCase("Test@test.com", 1, true)]
        [TestCase("Test@test.com", 2, false)]
        [TestCase("Test@test.com", 3, false)]
        public void UserControlService_ResetPassword_ReturnTrue_WhenCalledWithTimeNowIsWithinTimeLimit(string emailAddress, int userId, bool expected)
        {
            passWordResetModelOut.EmailAddress = emailAddress;
            passWordResetModelOut.TimeLimit = timelimit.AddDays(-userId);

            var passwordReset = new UserControlService(mockDatabaseService, mockUmbracoService, mockEmailService);

            passWordResetModel.EmailAddress = emailAddress;
            passWordResetModel.UserId = userId;

            var success = passwordReset.ResetPassword(passWordResetModel);

            Assert.AreEqual(expected, success);
        }

        [Test]
        [TestCase("Email", "", true)]
        [TestCase("", "Username", true)]
        [TestCase(" ", " ", false)]
        public void UserControlService_LookupUser_ReturnUmbracoUserModel_WhenCalled(string emailAddress, string username, bool expected)
        {
            umbracoUserModel.EmailAddress = emailAddress;
            umbracoUserModel.UserName = username;

            umbracoUserModelListOut.Add(umbracoUserModel);

            var lookupUser = new UserControlService(mockDatabaseService, mockUmbracoService, mockEmailService);

            findUserModel.EmailAddress = emailAddress;
            findUserModel.UserName = username;

            var user = lookupUser.LookupUsers(findUserModel);

            var success = false;

            if (user != null)
            {
                success = emailAddress == user.First().EmailAddress || username == user.First().UserName;
            }

            Assert.AreEqual(expected, success);
        }

        [Test]
        [TestCase(1, true, true)]
        [TestCase(1, false, true)]
        public void UserControlService_ToggleLock_ReturnTrue_WhenCalled(int userId, bool toggleLock, bool expected)
        {
            var disableUser = new UserControlService(mockDatabaseService, mockUmbracoService, mockEmailService);

            umbracoUserModel.UserId = userId;
            umbracoUserModel.UserLocked = toggleLock;

            var success = disableUser.ToggleLock(umbracoUserModel);

            Assert.AreEqual(expected, success);
        }

        [Test]
        [TestCase(1, 2, 2, "PageOne", 1, true)]
        [TestCase(1, 2, 3, "PageOne", 2, false)]
        public void PermissionsControlService_GetContentRoot_ReturnContentTreeViewModel_WhenCalled(int id, int pageId, int pageIdRoot, string pageName, int userId, bool expected)
        {
            permissionsModel.PermissionId = id;
            permissionsModel.PageId = pageId;
            permissionsModel.UserId = userId;
            permissionsModel.Created = DateTime.Now;

            permissionsModelsListOut.Add(permissionsModel);

            contentTreeViewModel.UserId = userId;
            contentTreeViewModel.PageId = pageIdRoot;
            contentTreeViewModel.PageName = pageName;

            contentTreeViewModelListOut.Add(contentTreeViewModel);

            var permissionsSservice = new PermissionsControlService(mockDatabaseService, mockUmbracoService, mockUserControlService);

            var page = permissionsSservice.GetContentRoot(contentTreeViewModel);

            Assert.AreEqual(expected, page.First().selected);
        }

        [Test]
        [TestCase(1, 1, 1, "PageOne", 1, 1, true)]
        [TestCase(1, 2, 3, "PageOne", 1, 1, false)]
        public void PermissionsControlService_GetContentChild_ReturnContentTreeViewModel_WhenCalled(int id, int pageId, int pageIdRoot, string pageName, int rootId, int userId, bool expected)
        {
            permissionsModel.PermissionId = id;
            permissionsModel.PageId = pageId;
            permissionsModel.UserId = userId;
            permissionsModel.Created = DateTime.Now;

            permissionsModelsListOut.Add(permissionsModel);

            contentTreeViewModel.RootId = rootId;
            contentTreeViewModel.PageId = pageIdRoot;
            contentTreeViewModel.PageName = pageName;
            contentTreeViewModel.UserId = userId;

            contentTreeViewModelListOut.Add(contentTreeViewModel);

            var permissionsSservice = new PermissionsControlService(mockDatabaseService, mockUmbracoService, mockUserControlService);

            var page = permissionsSservice.GetContentChild(contentTreeViewModel);

            Assert.AreEqual(expected, page.First().selected);
        }
    }
}