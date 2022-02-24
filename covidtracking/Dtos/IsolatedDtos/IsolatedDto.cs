using covidtracking.Entities;

namespace covidtracking.Dtos{
    public class IsolatedDto{
        public PotentialPatient potentialPatientDetails { get; set; }
        public Patient encounteredPatient { get; set; }

        public IsolatedDto(Patient patient, IPatient potentialPatient){
            potentialPatientDetails = new PotentialPatient(potentialPatient.GetFirstName(), 
                    potentialPatient.GetLastName(), potentialPatient.GetPhoneNumber());
            encounteredPatient = patient;
        }

    }
}