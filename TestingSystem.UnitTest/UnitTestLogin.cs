using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestingSystem.Areas.Admin.Controllers;
using TestingSystem.DataTranferObject;
using TestingSystem.Models;
using TestingSystem.Sevice;

namespace TestingSystem.UnitTest
{
    [TestClass]
    public class UnitTestLogin
    {
        private Mock<IUserService> _userService;
        _LoginController objController;

        private UserLogin myUser = new UserLogin();

        [TestInitialize]
        public void Initialize()
        {

            _userService = new Mock<IUserService>();
            objController = new _LoginController(_userService.Object);
        }
        [TestMethod]
        public void UserLogin()
        {
            myUser.userName = "admin1";
            myUser.password = "admin1";

            //Arrange
            int returnValues = 0;

            _userService.Setup(x => x.Login(myUser)).Returns(returnValues);

            //user
            var result = ((objController.Login(myUser) as ViewResult));

            //Assert
            Assert.AreEqual(result.ViewName, "Index");
            //Assert.AreEqual(returnValues,-1);
        }
    }
}
