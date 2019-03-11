﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestingSystem.Areas.Admin.Controllers;
using TestingSystem.Common;
using TestingSystem.Data.Infrastructure;
using TestingSystem.Data.Repositories;
using TestingSystem.DataTranferObject;
using TestingSystem.Models;
using TestingSystem.Sevice;

namespace TestingSystem.UnitTest
{
    [TestClass]
    public class UnitTestLogin
    {
        private Mock<IUserRepository> _mockRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private IUserService _mockService;
        private List<User> _listMock;

        [TestInitialize]    
        public void Initialize()
        {
            _mockRepository = new Mock<IUserRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockService = new UserService(_mockRepository.Object, _mockUnitOfWork.Object);
            _listMock = new List<User>()
        {
            new User()
            {
                UserId = 1,
                Avatar = "",
                Address = "",
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                Email = "email",
                Name = "a",
                Note = "a",
                Password = Encryptor.MD5Hash("admin"),
                UserName = "admin",
                Phone = "012",
                RoleId = 1,
                Status = 1
            },
            new User()
            {
                UserId = 2,
                Avatar = "",
                Address = "",
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                Email = "email",
                Name = "a",
                Note = "a",
                Password = Encryptor.MD5Hash("admin1"),
                UserName = "admin1",
                Phone = "012",
                RoleId = 1,
                Status = 0
            }
        };
        }

        [TestMethod]
        public void Test()
        {
        }


        [TestMethod]
        public void Service_Delete()
        {
            _mockRepository.Setup(m => m.DeleteUser(2)).Returns(1);
            var result = _mockService.DeleteUser(2);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void Service_Login()
        {
            UserLogin myUserLogin = new UserLogin();
            myUserLogin.userName = "admin1";
            myUserLogin.password = "admin1";

            _mockRepository.Setup(m => m.Login(myUserLogin)).Returns(2);
            var result = _mockService.Login(myUserLogin);
            Assert.AreEqual(2, result);
        }

        

        //[TestMethod]
        //public void Service_Update()
        //{
        //    Models.Question question = new Models.Question();
        //    question.QuestionID = 2;

        //    question.Content = "1";
        //    question.Image = null;
        //    question.Level = 1;
        //    question.CategoryID = 1;
        //    question.IsActive = true;
        //    question.CreatedBy = 1;
        //    question.ModifiedBy = 1;
        //    question.CreatedDate = DateTime.Now;
        //    question.ModifiedDate = DateTime.Now;

        //    _mockRepository.Setup(m => m.UpdateQuestion(question)).Returns(true);
        //    var result = _questionService.UpdateQuestion(question);
        //    Assert.AreEqual(true, result);
        //}
    }
}
