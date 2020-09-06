using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using EmailAPI.Controllers;
using EmailAPI.Models;
using EmailAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Moq;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace EmailAPI.Tests
{
    public class MailContollerPostTest
    {
        [Fact]
        public void MailsPost_ReturnsConflictResult()
        {
            //Arrange
            //Mocking ApplicationContext
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "EmailAPI.MoqDB")
                .Options;
            var context = new ApplicationContext(options);

            var emailPostFormList = new List<EmailPostForm>()
            {
                new EmailPostForm()
                {
                    
                },
                new EmailPostForm()
                {
                    subject = "test",
                },
                new EmailPostForm()
                {
                    body = "   "
                },
                new EmailPostForm()
                {
                    subject = "test",
                    body = "   "
                }, 
                new EmailPostForm()
                {
                    subject = "test",
                    recipients = new []{"okay@example.com"}
                }, 
                new EmailPostForm()
                {
                    recipients = new []{"okay@example.com"}
                }
            };

            //Act
            var controller = new MailsController(context, null);
            
            //Assert
            foreach (var emailPostForm in emailPostFormList)
            {
                var result = controller.Post(emailPostForm).Result;
                Assert.IsType<ConflictResult>(result);
            }
        }
        
        [Fact]
        public void MailsPost_OkResult()
        {
            //Arrange
            //Mocking ApplicationContext
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "EmailAPI.MoqDB")
                .Options;
            var context = new ApplicationContext(options);
            
            var emailPostForm = new EmailPostForm();
            emailPostForm.subject = "test";
            emailPostForm.body = "test";
            emailPostForm.recipients = new[] {"hello@example.com", "test@email.com", "Hi@yahoo@.com"};
            
            //Act
            var controller = new MailsController(context, null);
            
            //Assert
            var result = controller.Post(emailPostForm).Result;
            Assert.IsType<OkResult>(result);
        }
    }
}