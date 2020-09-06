using Microsoft.EntityFrameworkCore;

namespace EmailAPI.Models
{
    public class ApplicationContext : DbContext
    {
        /// <summary>
        /// Property to represent a set of EmailLog objects in a database.
        /// </summary>
        public DbSet<EmailLog> Emails { get; set; }
        
        /// <summary>
        /// Constructor for ApplicationContext.
        /// </summary>
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        { 
            Database.EnsureCreated();
        }
    }
}