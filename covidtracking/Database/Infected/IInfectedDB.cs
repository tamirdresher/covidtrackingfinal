namespace covidtracking.Database{
    public interface IInfectedDB{
        Task AddInfectedToDB(string id, DateTime infectedDateTime);
        Task RemoveInfectedFromDB(string id);
        HashSet<string> GetInfectedSince(DateTime since);
    }
}