namespace Breast_For_Baby.Models
{
    public class HomePageModel
    {
        public SessionDates Sessions { get; set; }

        public class SessionDates
        {
            public B4BEvent EducationalEvent1 { get; set; }
            public B4BEvent EducationalEvent2 { get; set; }
            public B4BEvent SupportGroup { get; set; }

            public class B4BEvent
            {
                public string Date { get; set; }
                public string Time { get; set; }
            }
        }
    }
}