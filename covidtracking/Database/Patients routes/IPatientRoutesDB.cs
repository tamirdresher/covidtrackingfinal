using covidtracking.Entities;

namespace covidtracking.Database{
        public interface IPatientRoutesDB
    {
        Task CreateVisitAsync(string id, Visit visit);
        Task<PatientRoute> GetPatientRouteByIdAsync(string id);
        Task InitPatientRoute(string id);
        Task ResetCollectionAsync();
        bool CheckValidVisitInput(Visit visit);
    }
}