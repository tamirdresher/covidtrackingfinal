using covidtracking.Entities;

namespace covidtracking.Database
{
    public interface IIsolatedDB
    {
        Task<Isolated> CreateIsolated(string key, string ecnountered);
        Task DeleteIsolatedAsync(string id);
        Task<Isolated> GetIsolatedByKeyAsync(string key);
        Task<List<string>> GetIsolatedEncounteredIdsAsync();
        Task<List<string>> GetIsolatedIdsAsync();
        Task ResetCollectionAsync();
        Task UpdateIsolatedEntityAsync(string oldKey, string newKey);
        Task<string> GetEncounteredPatientId(string id);
    }
}