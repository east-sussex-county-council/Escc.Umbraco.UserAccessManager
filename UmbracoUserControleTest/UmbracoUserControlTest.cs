using Moq;
using NUnit.Framework;
using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using UmbracoUserControl.Controllers;
using UmbracoUserControl.Models;
using UmbracoUserControl.Services;

namespace UmbracoUserControlTest
{
    [TestFixture]
    public class UmbracoUserControlTest
    {
        private IUserControlService userControlService;
        private IUmbracoService umbracoService;
        private IEmailService emailService;
        private IDatabaseService databaseService;

        private DateTime Timelimit;
        private PasswordResetModel passWordResetModel;
        private UmbracoUserModel umbracoUserModel;
        private PasswordResetModel passWordResetModelOut;

        [SetUp]
        public void UmbracoUserControlTestSetup()
        {
            userControlService = new Mock<IUserControlService>().Object;
            umbracoService = new Mock<IUmbracoService>().Object;
            emailService = new Mock<IEmailService>().Object;

            passWordResetModel = new PasswordResetModel();
            passWordResetModelOut = new PasswordResetModel();
            umbracoUserModel = new UmbracoUserModel();

            var mock = new Mock<IDatabaseService>();

            mock.Setup(x => x.GetResetDetails(passWordResetModel))
                .Returns(passWordResetModelOut);

            databaseService = mock.Object;

            Timelimit = DateTime.Now;
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
            var controller = new AdminController(umbracoService, userControlService);

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

            var controller = new AdminController(umbracoService, userControlService);

            var actionResult = controller.ResetPassword(passWordResetModel);

            Assert.IsInstanceOf(typeof(RedirectToRouteResult), actionResult, "Redirect to Success page if reset completes");
        }

        [Test]
        public void AdminController_ResetPassword_RedirectToAction_WhenCalledWithExpiredTimeLimit()
        {
            passWordResetModel.TimeLimit = DateTime.Now.AddDays(-2);

            var controller = new AdminController(umbracoService, userControlService);

            var actionResult = controller.ResetPassword(passWordResetModel);

            Assert.IsInstanceOf(typeof(RedirectToRouteResult), actionResult);
        }

        [Test]
        [TestCase("Test@test.com", 0, true)]
        [TestCase("Test@test.com", 1, true)]
        [TestCase("Test@test.com", 2, false)]
        [TestCase("Test@test.com", 3, false)]
        public void UserControlService_ResetPassword_ReturnTrue_WhenCalledWithTimeNowIsWithinTimeLimit(string EmailAddress, int UserId, bool expected)
        {
            passWordResetModelOut.EmailAddress = EmailAddress;
            passWordResetModelOut.TimeLimit = Timelimit.AddDays(-UserId);

            var passwordReset = new UserControlService(databaseService, umbracoService, emailService);

            passWordResetModel.EmailAddress = EmailAddress;
            passWordResetModel.UserId = UserId;

            bool success = passwordReset.ResetPassword(passWordResetModel);

            Assert.AreEqual(expected, success);
        }

        [Test]
        [TestCase(1, true, true)]
        [TestCase(1, false, true)]
        public void UserControleService_ToggleLock_ReturnTrue_WhenCalled(int UserId, bool toggleLock, bool expected)
        {
            var disableUser = new UserControlService(databaseService, umbracoService, emailService);

            umbracoUserModel.UserId = UserId;
            umbracoUserModel.Lock = toggleLock;

            bool success = disableUser.ToggleLock(umbracoUserModel);

            Assert.AreEqual(expected, success);
        }
    }
}