using covidtracking.Entities;

namespace covidtracking.Dtos{
    public class GetInfectedDto{
        public PotentialPatient potentialPatientDetails { get; set; }
        public Patient encounteredPatient { get; set; }
        public GetInfectedDto(Patient patient, PotentialPatient potentialPatient){
            potentialPatientDetails = potentialPatient;
            encounteredPatient = patient;
        }
    }
}