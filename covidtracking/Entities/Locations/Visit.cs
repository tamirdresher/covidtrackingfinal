using System;
using covidtracking.Dtos;

namespace covidtracking.Entities{
    public class Visit{
        public DateTime dateOfVisit { get; set; }
        public string siteName { get; set; }
        public Address siteAddress { get; set; }

        public Visit(CreateVisitDto createVisitDto){
            this.dateOfVisit = createVisitDto.dateOfVisit;
            this.siteName = createVisitDto.siteName;
            this.siteAddress = createVisitDto.siteAddress;
        }
    }
}