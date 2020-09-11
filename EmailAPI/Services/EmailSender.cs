using System;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using EmailAPI.Models;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;

namespace EmailAPI.Services
{
    /// <summary>
    /// Service for sending and logging emails
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private string _result = "";
        private string _exception = "";
        private string _host;
        private int _port;
        private string _fromAddress ;
        private string _password;
        
        /// <summary>
        /// Constructor of EmailSender
        /// </summary>
        /// <param name="configuration">Configuration from appsettings.json</param>
        /// <param name="serviceScopeFactory">A factory for creating instances of IServiceScope, which is used to create services within a scope.</param>
        public EmailSender(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
        }

        /// <summary>
        /// This method creates and sends email
        /// </summary>
        /// <remarks>
        /// MailKit.Net.Smtp is used to create and edit email.
        /// </remarks>
        /// <param name="emailLog">Passing values(Body, subject, recipients) using a model EmailLog. Used to return and add to the database.</param>
        public async Task<EmailLog> EmailSend(EmailLog emailLog)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                // I think this is correct based on:
                // https://docs.microsoft.com/ru-ru/ef/core/miscellaneous/configuring-dbcontext
                // Read the section: "Implicitly sharing DbContext instances across multiple threads via dependency injection"
                // However, I could be wrong! :)
                var db = scope.ServiceProvider.GetService<ApplicationContext>();
                
                ConfigurationSmtp(); //Configure host, port, login, password from IConfiguration
                
                MimeMessage message = new MimeMessage();
                message.Subject = emailLog.Subject;
                message.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
                {
                    Text = emailLog.Body
                };
                message.From.Add(new MailboxAddress(_fromAddress, _fromAddress));
                message.To.Add(new MailboxAddress(emailLog.Recipients, emailLog.Recipients));
                
                using (var smtp = new SmtpClient())
                {
                    smtp.AuthenticationMechanisms.Remove("XOAUTH2");
                    await TryConnectSmtp(smtp, message); //TryConnection -> TryAuthentication -> TryToSend. If everything is ok, then the message is sent.
                    emailLog.From = _fromAddress;
                    emailLog.DateOfSend = DateTime.Now;
                    emailLog.Result = _result;
                    emailLog.FailedMessage = _exception;
                }
                await db.Emails.AddAsync(emailLog);
                await db.SaveChangesAsync();
                _exception = "";
            }
            return emailLog;
        }

        private void ConfigurationSmtp()
        {
            _host = _configuration.GetValue<string>("Smtp:Server"); 
            _port = _configuration.GetValue<int>("Smtp:Port");
            _fromAddress = _configuration.GetValue<string>("Smtp:FromAddress");
            _password = _configuration.GetValue<string>("Smtp:Password");
        }

        private async Task TryConnectSmtp(SmtpClient smtp, MimeMessage message)
        {
            try
            {
                await smtp.ConnectAsync(_host, _port);
                _result = "Ok";
            }
            catch (SmtpCommandException ex)
            {
                _exception = $" {ex.StatusCode} Error trying to connect: {ex.Message}";
                _result = "Failed";
            }
            catch (SmtpProtocolException ex)
            {
                _exception = $"Protocol error while trying to connect: {ex.Message}";
                _result = "Failed";
            }
            finally
            {
                if (_result == "Ok")
                {
                    await TryAuthenticate(smtp, message);
                }
            }
        }
        
        private async Task TryAuthenticate(SmtpClient smtp, MimeMessage message)
        {
            try
            {
                await smtp.AuthenticateAsync(_fromAddress, _password);
                _result = "Ok";
            }
            catch (AuthenticationException ex)
            {
                _exception = "Invalid user name or password.";
                _result = "Failed";
            } 
            catch (SmtpCommandException ex)
            {
                _exception = $"{ex.StatusCode} Error trying to authenticate: {ex.Message}";
                _result = "Failed";
            } 
            catch (SmtpProtocolException ex) 
            {
                _exception = $"Protocol error while trying to authenticate: {ex.Message}";
                _result = "Failed";
            }
            finally
            {
                if (_result == "Ok")
                {
                    await TryToSend(smtp, message);
                }
            }
        }   
        
        private async Task TryToSend(SmtpClient smtp, MimeMessage message)
        {
            try 
            {
                await smtp.SendAsync(message);
                _result = "Ok";
            } 
            catch (SmtpCommandException ex) 
            {
                _exception = $"{ex.StatusCode} Error sending message: {ex.Message}";
                _result = "Failed";
            } 
            catch (SmtpProtocolException ex) 
            {
                _exception = $"Protocol error while sending message: {ex.Message}";
                _result = "Failed";
            }
            
            await smtp.DisconnectAsync (true);
        }
    }
}

        
