using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Breast_For_Baby.Data;
using Dapper;

namespace Breast_For_Baby.Controllers
{
    public class AdminController : Controller
    {
        private readonly string[] _ips;
        public AdminController()
        {
            _ips = ConfigurationManager.AppSettings["Admin_IPs"].Split(',');
        }

        private bool IsAllowed(HttpRequestBase request)
        {
            var requestorId = request.UserHostAddress;

            return _ips.Any(ip => ip == requestorId);
        }

        [Route("admin")]
        public ActionResult Index()
        {
            if (!IsAllowed(Request))
            {
                return RedirectToAction("Oops", "Error");
            }

            var events = Event.GetAllNonDeleted().OrderBy(e => e.StartDateTime).ToList();

            var mapped = events.Select(e => new EventViewModel
            {
                Id = e.Id,
                Name = e.Name,
                DateDescription = e.Date,
                TimeDescription = e.Time,
                StartDateTime = e.StartDateTime,
                Deleted = false,
                CurrentlyLive = false
            }).ToList();

            foreach (var eventName in EventName.ListAll)
            {
                var idOfNext = Event.GetNext(eventName);

                if (idOfNext.HasValue)
                {
                    mapped.Single(e => e.Id == idOfNext.Value).CurrentlyLive = true;
                }
            }

            return View(mapped);
        }

        [Route("createEvent")]
        public ActionResult Add(AddEventModel model)
        {
            if (!IsAllowed(Request))
            {
                return RedirectToAction("Oops", "Error");
            }

            var dto = new EventDto
            {
                Name = model.Name,
                Date = model.Date,
                Time = model.Time,
                StartDateTime = model.DateTime,
                Deleted = false
            };

            const string addEventSql = "INSERT INTO Event (Name, Date, Time, StartDateTime, Deleted) VALUES (@Name, @Date, @Time, @StartDateTime, @Deleted)";

            using (var conn = Store.CreateOpenConnection())
            {
                conn.Execute(addEventSql, dto);
            }

            return RedirectToAction("Index");
        }

        [Route("deleteEvent")]
        public ActionResult Delete(DeleteEventModel model)
        {
            if (!IsAllowed(Request))
            {
                return RedirectToAction("Oops", "Error");
            }

            using (var conn = Store.CreateOpenConnection())
            {
                conn.Execute("UPDATE EVENT SET Deleted = 1 WHERE Id = @Id", new { model.Id });
            }

            return RedirectToAction("Index");
        }
    }

    public class EventViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DateDescription { get; set; }
        public string TimeDescription { get; set; }
        public DateTime StartDateTime { get; set; }
        public bool Deleted { get; set; }
        public bool CurrentlyLive { get; set; }
    }

    public class AddEventModel
    {
        public string Name { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public DateTime DateTime { get; set; }
    }

    public class DeleteEventModel
    {
        public int Id { get; set; }
    }

    public class EventDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public DateTime StartDateTime { get; set; }
        public bool Deleted { get; set; }
    }
}