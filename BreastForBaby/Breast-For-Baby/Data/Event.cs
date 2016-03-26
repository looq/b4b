using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace Breast_For_Baby.Data
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public DateTime StartDateTime { get; set; }
        public bool Deleted { get; set; }

        private const int DisplayOffsetHours = 3;

        public static List<Event> GetAllNonDeleted()
        {
            using (var conn = Store.CreateOpenConnection())
            {
                var events =
                    conn.Query<Event>("SELECT Id,Name,Date,Time,StartDateTime FROM EVENT WHERE Deleted = 0")
                        .OrderBy(e => e.StartDateTime)
                        .ToList();

                return events;
            }
        }

        public static int? GetNext(string eventName)
        {
            using (var conn = Store.CreateOpenConnection())
            {
                var events = conn.Query<Event>("SELECT * FROM Event WHERE Name = @Name AND Deleted = 0", new { Name = eventName }).ToList();

                if (!events.Any())
                {
                    return null;
                }

                var ordered = events.OrderBy(e => e.StartDateTime);

                if (!events.Any(e => e.StartDateTime > DateTime.UtcNow.AddHours(-DisplayOffsetHours)))
                {
                    return ordered.Last().Id;
                }

                var futureEvents = ordered.Where(e => e.StartDateTime > DateTime.UtcNow.AddHours(-DisplayOffsetHours));

                return futureEvents.First().Id;
            }
        }
    }

    public static class EventName
    {
        public const string EducationalClass1 = "Educational Class Part 1";
        public const string EducationalClass2 = "Educational Class Part 2";
        public const string SupportGroup = "Support Group";

        public static List<string> ListAll => new List<string>
        {
            EducationalClass1,
            EducationalClass2,
            SupportGroup
        };
    }
}