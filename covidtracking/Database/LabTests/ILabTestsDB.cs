using covidtracking.Entities;

namespace covidtracking.Database{
    public interface ILabTestsDB{
        Task<int> CreateTestAsync(LabTestResult labTest);
        Task InitLabTestsAsync(string govtId);
        Task ResetCollectionAsync();
        Task<LabTests> GetLabTests(string id);
    }
}