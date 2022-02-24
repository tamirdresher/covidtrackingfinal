using covidtracking.Entities;

namespace covidtracking.Database
{
    public interface IPotentialPatientsDB
    {
        Task CreatePotentialPatientAsync(PotentialPatient potentialPatient);
        Task<PotentialPatient> GetPotentialPatientByIdAsync(string key);
        Task DeletePotentialPatient(string key);
        Task<bool> CheckIfPotentialPatientExistsAsync(string key);
        Task ResetCollectionAsync();
        HashSet<string> CreateHashSet();
        bool CheckValidPotentialPatientInput(PotentialPatient potentialPatient);
    }
}