using System.Threading.Tasks;
using EmailAPI.Models;

namespace EmailAPI.Services
{
    /// <summary>
    /// This is interface for EmailSender. Used for dependency injection
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Public method used by the EmailSender class
        /// </summary>
        /// <param name="emailLog">Passing values(Body, subject, recipients) using a model EmailLog</param>
        Task<EmailLog> EmailSend(EmailLog emailLog);
    }
}