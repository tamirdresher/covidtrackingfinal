using covidtracking.Entities;

namespace covidtracking.Dtos{
    public class PatientInformationDto{
        public Patient patientDetails { get; set; }
        public bool isCovidPositive { get; set; }
        public List<LabTestResult> labTestResults { get; set; }

        public PatientInformationDto(Patient patient, LabTests labtests){
            patientDetails = patient;
            isCovidPositive = patient.isCovidPositive;
            labTestResults = labtests.labTestResults;
        }
    }
}