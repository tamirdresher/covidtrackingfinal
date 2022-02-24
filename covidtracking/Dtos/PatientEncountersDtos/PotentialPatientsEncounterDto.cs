using covidtracking.Entities;

namespace covidtracking.Dtos{
    public class PotentialPatientsEncounterDto{
        public PotentialPatient potentialPatientDetails { get; set; }
        public Patient encounteredPatient { get; set; }
        public PotentialPatientsEncounterDto(Patient patient, PotentialPatient potentialPatient){
            potentialPatientDetails = potentialPatient;
            encounteredPatient = patient;
        }
    }
}