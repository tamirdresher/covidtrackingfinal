using System.Collections;
using covidtracking.Dtos;
using covidtracking.Entities;

namespace covidtracking.Database{
    public interface IPatientsEncountersDB{
        Task AddPatientEncounterAsync(string id, PotentialPatient potentialPatient);
        Task<PatientEncounter> GetPatientEncountersAsync(string id);
        Task InitPatientEncountersAsync(string id);
        Task ResetCollectionAsync();
        IEnumerable<PotentialPatientsEncounterDto> GetPotentialPatientsEncounters(HashSet<string> potentialPatientsKeys, Hashtable patients);
    }
}