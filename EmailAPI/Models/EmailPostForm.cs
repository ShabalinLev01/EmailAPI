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
        public string subject { get; set; }
        
        /// <summary>
        /// Body of email.
        /// </summary>
        public string body { get; set; }
        
        /// <summary>
        /// Array of recipient addresses.
        /// </summary>
        public string[] recipients { get; set; }
    }
}