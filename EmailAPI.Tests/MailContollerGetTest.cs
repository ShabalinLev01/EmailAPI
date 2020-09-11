using System;
using System.Linq;
using EmailAPI.Controllers;
using EmailAPI.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace EmailAPI.Tests
{
    public class MailContollerGetTest
    {

        [Fact]
        public void MailsGet_returnJson_WhenInDB_3_Rows()
        {
            //Arrange
            //Mocking ApplicationContext
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "EmailAPI.MoqDB")
                .Options;
            var context = new ApplicationContext(options);

            context.Add(new EmailLog()
            {
                From = "from@example.com",
                Recipients = "to@example.com",
                Subject = "Test",
                Body = "Test",
                DateOfSend = DateTime.Now,
                Result = "Ok",
                FailedMessage = ""
            });
            context.Add(new EmailLog()
            {
                From = "from@example.com",
                Recipients = "to@example.com",
                Subject = "Test",
                Body = "Test",
                DateOfSend = DateTime.Now,
                Result = "Failed",
                FailedMessage = "MailboxBusy"
            });
            context.Add(new EmailLog()
            {
                From = "from@example.com",
                Recipients = "to2@example.com",
                Subject = "Test",
                Body = "Test",
                DateOfSend = DateTime.Now,
                Result = "Ok",
                FailedMessage = ""
            });

            context.SaveChanges();
            
            var settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Error = (sender, args) =>
                {
                    args.ErrorContext.Handled = true;
                },
            };
            
            var myEntity = context.Emails.Select(x=>x).ToList();
            var resultJson = JsonConvert.SerializeObject(myEntity, settings);
            
            //Act
            var controller = new MailsController(context, null);
            
            //Assert
            var result = controller.Get();
            Assert.Equal(resultJson, result.Result);
        }
        
        [Fact]
        public void MailsGet_returnJson_WhenDB_is_Empty()
        {
            //Arrange
            //Mocking ApplicationContext
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "EmailAPI.MoqDB")
                .Options;
            var context = new ApplicationContext(options);

            var settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Error = (sender, args) =>
                {
                    args.ErrorContext.Handled = true;
                },
            };
            
            var myEntity = context.Emails.Select(x=>x).ToList();
            var resultJson = JsonConvert.SerializeObject(myEntity, settings);
            
            //Act
            var controller = new MailsController(context, null);
            
            //Assert
            var result = controller.Get();
            Assert.Equal(resultJson, result.Result);
        }
    }
}