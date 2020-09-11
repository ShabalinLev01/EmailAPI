namespace EmailAPI.Models
{
    /// <summary>
    /// This is a class that contains several properties for the Json request(Post in MailsContoller) submission form.
    /// </summary>
    public class EmailPostForm
    {
        /// <summary>
        /// Subject of email.
        /// </summary>
        public string Subject { get; set; }
        
        /// <summary>
        /// Body of email.
        /// </summary>
        public string Body { get; set; }
        
        /// <summary>
        /// Array of recipient addresses.
        /// </summary>
        public string[] Recipients { get; set; }
    }
}