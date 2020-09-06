using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmailAPI.Models;
using EmailAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EmailAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MailsController : Controller
    {
        private readonly ApplicationContext _db;
        private readonly EmailSender _emailSender;

        /// <summary>
        /// Constructor of MailsContoller
        /// </summary>
        /// <param name="db">DbContext</param>
        /// <param name="emailSender">EmailSender service for multithreading</param>
        public MailsController( ApplicationContext db, EmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }

        /// <summary>
        /// List of all sent messages saved in the database in json format
        /// </summary>
        /// <remarks>
        /// This query will output all sent and unsent emails from the database in json format
        /// </remarks>
        [HttpGet]
        public async Task<string> Get()
        {
            var settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Error = (sender, args) =>
                {
                    args.ErrorContext.Handled = true;
                },
            };
            
            var myEntity = _db.Emails.Select(x=>x).ToList();
            return JsonConvert.SerializeObject(myEntity, settings);
        }

        ///  <summary>
        ///  The request forms an email and sends it using the EmailSender service
        ///  </summary>
        ///  <param name="email">Contains: subject, body, recipients</param>
        ///  <param name="subject">Subject of mail</param>
        ///  <param name="body">Body of mail</param>
        ///  <param name="recipients">Recipients of mail</param>
        ///  <remarks>
        ///  NOTICE!
        ///  Errors related to smtp will be saved to the database
        ///  and don't affect the controller's execution in anyway!
        ///  NOTICE!
        /// 
        /// A separate thread of the "EmailSend" method is created for each address
        ///  
        ///  Sample request:
        ///      
        ///      {
        ///          "subject": "string",
        ///          "body": "string",
        ///          "recipients": ["example@gmail.com", "example2@gmail.com", "example3@gmail.com"]
        ///      }
        /// 
        ///  </remarks>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EmailPostForm email)
        {
            if (email.body == null || email.recipients == null || email.subject == null)
            {
                return Conflict();
            }
            else
            {
                foreach (var recipient in email.recipients)
                {
                    Thread myThread = new Thread(() => _emailSender.EmailSend(recipient, email.body, email.subject));
                    myThread.Start();
                }
            }
            return Ok();
        }
    }
}
    