using covidtracking.Entities;

namespace covidtracking.Dtos{
    public class GetPatientDto{
        public string govtId { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public DateTime birthDate { get; set; }
            public string phoneNumber { get; set; }
            public string email { get; set; }
            public Address address  { get; set; }
            public bool isCovidPositive { get; set; }
            public string? infectedByPatientID { get; set; }

            public GetPatientDto(Patient patient){
            govtId = patient.govtId;
            firstName = patient.firstName;
            lastName = patient.lastName;
            birthDate = patient.birthDate;
            phoneNumber = patient.phoneNumber;
            email = patient.email;
            address = patient.address;
            isCovidPositive = patient.isCovidPositive;
            infectedByPatientID = patient.infectedByPatientID;
        }
    }
}