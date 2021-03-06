﻿using System;
using System.Collections.Generic;
using EmailAPI.Models;
using EmailAPI.Services;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace EmailAPI.Tests
{
    public class EmailSenderTest
    {
        public Dictionary<string, string> ConfigurationSMTPforTest()
        {
            var myConfiguration = new Dictionary<string, string>
            {
                {"Smtp:Server", "smtp.client.example"},
                {"Smtp:Port", "25"},
                {"Smtp:FromAddress", "email@example.com"},
                {"Smtp:Password", "password"}
            };
            return myConfiguration;
        }
 
        // Unfortunately, I have not found the correct way to unit test smtpclient.
        // This test departs from unit testing standards.
        // Uses a real smtpclient.
        
        [Fact]
        public void EmailSenderTestForResult_NEEDTOSETUP() //Setup in ConfigurationSMTPforTest and in Assert.Equal change your expected value
        {
            //Arrange
            Mock<SmtpClient> smtpClient = new Mock<SmtpClient>();
            
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(ConfigurationSMTPforTest())
                .Build();
            
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "EmailAPI.MoqDB")
                .Options;

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(ApplicationContext)))
                .Returns(new ApplicationContext(options));

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);

            //Act
            var service = new EmailSender(configuration, serviceScopeFactory.Object);
            EmailLog emailLog = new EmailLog();
            emailLog.Recipients = "shabalinlev01gmail.com";
            emailLog.Body = "body";
            emailLog.Subject = "subject";
            var result = service.EmailSend(emailLog);
            
            //Assert
            Assert.Equal("Failed", result.Result.Result); //FOR STANDART CONFIG IN APPSETTINGS.JSON
        }
    }
}