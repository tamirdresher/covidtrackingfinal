using covidtracking.Entities;

namespace covidtracking.Dtos{
    public class CreateVisitDto{
        public DateTime dateOfVisit { get; set; }
        public string siteName { get; set; }
        public Address siteAddress { get; set; }
    }
}