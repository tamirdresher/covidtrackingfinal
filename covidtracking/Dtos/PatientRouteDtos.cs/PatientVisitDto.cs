using covidtracking.Entities;

namespace covidtracking.Dtos{
    public class PatientVisitDto{
        public string? id { get; set; }
        public DateTime dateOfVisit { get; set; }
        public string siteName { get; set; }
        public Address siteAddress { get; set; }
    
        public PatientVisitDto(Visit visit, string id){
            this.id = id;
            dateOfVisit = visit.dateOfVisit;
            siteName = visit.siteName;
            siteAddress = visit.siteAddress;
        } 
    }   
}