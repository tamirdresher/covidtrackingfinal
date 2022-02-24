using System.Runtime.ConstrainedExecution;
using covidtracking.Dtos;

namespace covidtracking.Entities{
    public class PotentialPatient : IPatient
    {
        public string key { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phoneNumber { get; set; }

        public PotentialPatient(CreatePotentialPatientDto createPotentialPatientDto){
            firstName = createPotentialPatientDto.firstName;
            lastName = createPotentialPatientDto.lastName;
            phoneNumber = createPotentialPatientDto.phoneNumber;
            key = firstName+lastName+phoneNumber;
        }

        public PotentialPatient(string fname, string lname, string phone){
            firstName = fname;
            lastName = lname;
            phoneNumber = phone;
            key = firstName+lastName+phoneNumber;
        }

        public PotentialPatient(Patient patient){
            firstName = patient.firstName;
            lastName = patient.lastName;
            phoneNumber = patient.phoneNumber;
            key = firstName+lastName+phoneNumber;
        }

        public string GetKey(){
            return firstName+lastName+phoneNumber;
        }

        public string GetFirstName()
        {
            return firstName;
        }

        public string GetLastName()
        {
            return lastName;
        }

        public string GetPhoneNumber()
        {
            return phoneNumber;
        }

        public string GetId()
        {
            return firstName+lastName+phoneNumber;
        }
    }
}