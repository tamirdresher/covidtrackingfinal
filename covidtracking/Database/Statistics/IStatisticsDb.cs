using covidtracking.Entities;

namespace covidtracking.Database
{
    public interface IStatisticsDb
    {
        void InitStatisticsDb();
        Statistics GetCurrentStatistics();
        void UpdateHealed();
        void UpdateIsolated(char operation);
        void UpdateCityInfected(string city, char operation);
        bool AddCityToDb(string city);
        void UpdateStatistics(Statistics newStats);
        Task ResetCollectionAsync();
    }
}