using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using EmailAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmailAPI.Services
{
    public class EmailSender
    {
        private static IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
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
        /// System.Net.Mail is used to create and edit email.
        /// </remarks>
        /// <param name="recipient">Recipient of email</param>
        /// <param name="body">Body of email</param>
        /// <param name="subject">Subject of email</param>
        public async Task<EmailLog> EmailSend(string recipient, string body, string subject)
        {
            EmailLog emailLogdb;
            using (var scope = _serviceScopeFactory.CreateScope()) 
            {
                var dbContext = scope.ServiceProvider.GetService<ApplicationContext>();
                using (MailMessage message = new MailMessage())
                {
                    string exception = "";
                    string result = "";
                    string host = _configuration.GetValue<string>("Smtp:Server");
                    int port = _configuration.GetValue<int>("Smtp:Port");
                    string fromAddress = _configuration.GetValue<string>("Smtp:FromAddress");
                    string password = _configuration.GetValue<string>("Smtp:Password");
                    bool enableSsl = _configuration.GetValue<bool>("Smtp:EnableSSL");
                    
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;
                    message.From = new MailAddress(fromAddress);
                    
                    using (SmtpClient smtp = new SmtpClient(host, port))
                    {
                        smtp.Credentials = new NetworkCredential(fromAddress, password);
                        smtp.EnableSsl = enableSsl;
                        
                        try
                        {
                            message.To.Add(recipient);
                            await smtp.SendMailAsync(message);
                            result = "Ok";
                        }
                        catch (SmtpException e)
                        {
                            exception = e.StatusCode.ToString();
                            result = "Failed";
                            Console.WriteLine("Error: {0}", exception);
                        }
                        catch (Exception e)
                        {
                            exception = e.Message;
                            result = "Failed";
                            Console.WriteLine("Error: {0}", exception);
                        }
                        finally
                        {
                            emailLogdb = new EmailLog()
                            {
                                From = fromAddress,
                                Recipients = recipient,
                                Subject = message.Subject,
                                Body = message.Body,
                                DateOfSend = DateTime.Now,
                                Result = result,
                                FailedMessage = exception
                            };
                            await dbContext.Emails.AddAsync(emailLogdb);
                            await dbContext.SaveChangesAsync();
                        }
                    }
                }
            }
            return emailLogdb;
        }
    }
}

        
