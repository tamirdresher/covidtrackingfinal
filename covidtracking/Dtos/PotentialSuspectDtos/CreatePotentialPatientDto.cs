using covidtracking.Entities;

namespace covidtracking.Dtos
{
    public class CreatePotentialPatientDto
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phoneNumber { get; set; }
    }
}