using covidtracking.Dtos;
using covidtracking.Entities;

namespace covidtracking.Utilities
{
    public class Adapters
    {
        public Patient PotentialPatientToPatient(CreatePatientDto createPatientDto, string infectedBy)
        {
            Patient convertedPatient = new Patient(createPatientDto);
            convertedPatient.infectedByPatientID = infectedBy;
            return convertedPatient;
        }
        public PotentialPatient PotentialPatientToPatient(Patient patient)
        {
            PotentialPatient convertedPatient = new PotentialPatient(patient);
            return convertedPatient;
        }
    }
}