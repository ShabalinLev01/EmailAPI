using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IEmailSender _emailSender;

        /// <summary>
        /// Constructor of MailsContoller
        /// </summary>
        /// <param name="db">DbContext</param>
        /// <param name="emailSender">EmailSender service for multithreading</param>
        public MailsController( ApplicationContext db, IEmailSender emailSender)
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
            var myEntity = _db.Emails.ToList();
            return JsonConvert.SerializeObject(myEntity);
        }

        ///  <summary>
        ///  The request forms an email and sends it using the EmailSender service
        ///  </summary>
        ///  <param name="email">Contains: subject, body, recipients[]</param>
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
            var mapper  = new MapperConfiguration(cfg => cfg.CreateMap<EmailPostForm, EmailLog>()
                    .ForMember("Body", opt => opt.MapFrom(c => c.Body))
                    .ForMember("Subject", opt => opt.MapFrom(c => c.Subject))
                    .ForMember("Recipients", opt => opt.MapFrom(c => c.Recipients)))
                .CreateMapper();
            Parallel.ForEach(email.Recipients, async recipient =>
            {
                EmailLog emailLog = mapper.Map<EmailPostForm, EmailLog>(email);
                emailLog.Recipients = recipient;
                await _emailSender.EmailSend(emailLog);
            });
            return Ok();
        }
    }
}
    