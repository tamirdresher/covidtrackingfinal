 using covidtracking.Dtos;
 using covidtracking.Entities;

 namespace covidtracking.Utilities{
     public static class Exstensions{
         public static GetPatientDto PatientAsGetPatientDto(this Patient patient){
             return new GetPatientDto(patient);
         }

         public static PatientRouteDto PatientRouteAsDto(this PatientRoute patientRoute){
             return new PatientRouteDto(patientRoute);
         }

         public static PatientInformationDto PatientAsPatientInformationDto(this Patient patient, LabTests labTests){
             PatientInformationDto patientInformationDto =  new PatientInformationDto(patient, labTests);
             return patientInformationDto;
         }

         public static PatientEncountersDto PatientEncountersAsDto(this PatientEncounter patientEncounter){
             return new PatientEncountersDto(patientEncounter);
         }
     }
}