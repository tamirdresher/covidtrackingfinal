using covidtracking.Entities;

namespace covidtracking.Dtos
{
    public class PotentialToPatientDto
    {
        public string govtId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public DateTime birthDate { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public Address address { get; set; }
        public bool isCovidPositive { get; set; }
        public int houseResidentsAmount { get; set; }
        public string infectedByPatientID { get; set; }
    }
}