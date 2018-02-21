using Moq;
using NUnit.Framework;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Infrastructure.Abstract;
using SportsStore.WebUI.Models;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
    [TestFixture]
    public class UnitTest2
    {
        [Test]
        public void Can_Login_With_Valid_Credentials()
        {
            // Arrange - create a mock authentication provider
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(authProvider => authProvider.Authenticate("admin", "12345")).Returns(true);

            // Arrange - create the view model
            LoginViewModel loginViewModel = new LoginViewModel() { UserName = "admin", Password = "12345" };

            // Arrange - create the controller
            AccountController accountController = new AccountController(mock.Object);

            // Act - authenticate using valid credentials
            ActionResult result = accountController.Login(loginViewModel, "/MyURL");

            //Assert
            Assert.IsInstanceOf(typeof(RedirectResult), result);
            Assert.AreEqual("/MyURL", ((RedirectResult)result).Url);
        }

        [Test]
        public void Cannot_Login_With_Invalid_Credentials()
        {
            //Arrange - create a mock authentication provider
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(authProvider => authProvider.Authenticate("admin", "54321")).Returns(false);

            // Arrange - create the view model
            LoginViewModel loginViewModel = new LoginViewModel() { UserName = "admin", Password = "54321" };

            //Arrange - create the controller
            AccountController accountController = new AccountController(mock.Object);

            //Act - autenticate using invalid credentials
            ActionResult result = accountController.Login(loginViewModel, "/MyURL");

            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
        }
    }
}
