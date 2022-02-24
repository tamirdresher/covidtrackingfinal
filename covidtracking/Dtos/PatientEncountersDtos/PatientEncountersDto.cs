using covidtracking.Entities;

namespace covidtracking.Dtos{
    public class PatientEncountersDto{
        public string id { get; init; }
        public List<PotentialPatient> potentialPatientsEncountered { get; set; }

        public PatientEncountersDto(PatientEncounter patientEncounter){
            id = patientEncounter.id;
            potentialPatientsEncountered = patientEncounter.potentialPatientsEncountered;
        }
    }
}