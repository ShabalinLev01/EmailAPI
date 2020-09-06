using System;

namespace EmailAPI.Models
{
    /// <summary>
    /// This is a class that contains several properties for logging
    /// the result of sending a message to the database.
    /// </summary>
    public class EmailLog
    {
        public int Id { get; set; }

        /// <summary>
        /// Sender address, specify in appsettings.json.
        /// </summary>
        public string From { get; set; }
        
        /// <summary>
        /// Address of the recipient.
        /// </summary>
        public string Recipients { get; set; }
        
        /// <summary>
        /// Subject of email.
        /// </summary>
        public string Subject { get; set; }
        
        /// <summary>
        /// Body of email.
        /// </summary>
        public string Body { get; set; }
        
        /// <summary>
        /// Date and time the email was sent.
        /// </summary>
        public DateTime DateOfSend { get; set; }
        
        /// <summary>
        /// Submission result, response contains "OK" or "Failed".
        /// </summary>
        public string Result { get; set; }
        
        /// <summary>
        /// Description of the message sending error,
        /// the response contains a description of the error code.
        /// </summary>
        public string FailedMessage { get; set; }
    }
}