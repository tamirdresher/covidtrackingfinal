using System.Collections;
using covidtracking.Entities;

namespace covidtracking.Database
{
    public interface IPatientsDB
    {
        Task CreatePatientAsync(Patient patient);
        Task DeletePatientAsync(string id);
        Task<Patient> GetPatientAsync(string id);
        Task<List<Patient>> GetPatientsAsync();
        Task ResetCollectionAsync();
        Task MakeSickAsync(Patient patient);
        Task<Hashtable> CreateHashTable();
        bool CheckValidPatientInput(Patient patient);
        Task<List<Patient>> GetPatientsWithIdsAsync(HashSet<string> infectedIds);
    }
}