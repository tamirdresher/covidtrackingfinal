using covidtracking.Entities;

namespace covidtracking.Dtos
{
    public class PatientRouteDto
    {
        public string id { get; init; }
        public List<Visit> route { get; set; }
        public PatientRouteDto(PatientRoute patientRoute)
        {
            id = patientRoute.id;
            route = patientRoute.route;
        }
    }
}