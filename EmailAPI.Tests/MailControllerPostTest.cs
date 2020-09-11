using EmailAPI.Controllers;
using EmailAPI.Models;
using EmailAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EmailAPI.Tests
{
    public class MailContollerPostTest
    {

        [Fact]
        public void MailsPost_OkResult()
        {
            //Arrange
            Mock<IEmailSender> emailSender = new Mock<IEmailSender>();
            
            //Mocking ApplicationContext
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "EmailAPI.MoqDB")
                .Options;
            var context = new ApplicationContext(options);
            
            var emailPostForm = new EmailPostForm();
            emailPostForm.Subject = "test";
            emailPostForm.Body = "test";
            emailPostForm.Recipients = new[] {"hello@example.com", "test@email.com", "Hi@yahoo@.com"};
            
            //Act
            var controller = new MailsController(context, emailSender.Object);
            
            //Assert
            var result = controller.Post(emailPostForm).Result;
            Assert.IsType<OkResult>(result);
        }
    }
}