using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using Breast_For_Baby.Data;
using Breast_For_Baby.Models;

namespace Breast_For_Baby.Controllers
{
    public class BreastForBabyController : Controller
    {
        public ActionResult Index()
        {
            var model = GetHomepageModel();

            return View(model);
        }

        private HomePageModel GetHomepageModel()
        {
            var events = Event.GetAllNonDeleted();

            var supportGroupId = Event.GetNext(EventName.SupportGroup);
            var class1Id = Event.GetNext(EventName.EducationalClass1);
            var class2Id = Event.GetNext(EventName.EducationalClass2);

            if (!supportGroupId.HasValue || !class1Id.HasValue || !class2Id.HasValue)
            {
                return null;
            }

            var supportGroup = events.Single(e => e.Id == supportGroupId);
            var class1 = events.Single(e => e.Id == class1Id);
            var class2 = events.Single(e => e.Id == class2Id);
            
            return new HomePageModel
            {
                Sessions = new HomePageModel.SessionDates
                {
                    SupportGroup = new HomePageModel.SessionDates.B4BEvent
                    {
                        Date = supportGroup.Date,
                        Time = supportGroup.Time
                    },
                    EducationalEvent1 = new HomePageModel.SessionDates.B4BEvent
                    {
                        Date = class1.Date,
                        Time = class1.Time
                    },
                    EducationalEvent2 = new HomePageModel.SessionDates.B4BEvent
                    {
                        Date = class2.Date,
                        Time = class2.Time
                    }
                }
            };
        }

        [Route("book")]
        [HttpGet]
        public ActionResult Book()
        {
            return View();
        }

        [Route("book")]
        [HttpPost]
        public ActionResult Book(BookInputModel model)
        {
            var emailOn = bool.Parse(ConfigurationManager.AppSettings["Email_On"]);

            if (!emailOn)
            {
                return View("Thanks");
            }

            var emailTo = ConfigurationManager.AppSettings["Email_To"];
            var emailFrom = ConfigurationManager.AppSettings["Email_From"];
            var smtpPassword = ConfigurationManager.AppSettings["Email_Smpt_Password"];

            if (string.IsNullOrWhiteSpace(model.Situation))
            {
                model.Situation = "{ none provided }";
            }

            var mail = new MailMessage
            {
                From = new MailAddress(emailFrom)
            };

            var emails = emailTo.Split(',');

            foreach (var email in emails)
            {
                mail.To.Add(email);
            }

            mail.Subject = "New Website Enquiry";
            mail.Body = $@"There is a new website enquiry!

Name : {model.Name}
Contact: {model.ContactMethod}
Details: { model.Situation }";

            var smtp = new SmtpClient("auth.smtp.1and1.co.uk")
            {
                Credentials = new NetworkCredential(emailFrom, smtpPassword)
            };

            smtp.Send(mail);

            return View("Thanks");
        }
    }
}