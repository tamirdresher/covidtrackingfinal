using covidtracking.Entities;
using MongoDB.Driver;

namespace covidtracking.Database{
    public class MongoDBLabTestsModel : ILabTestsDB
    {
        //Database access constants
        private const string DatabaseName = "covidtracking";
        private const string CollectionName = "labtests";

        //Properties
        private readonly IMongoCollection<LabTests> labtestsCollection;
        private readonly FilterDefinitionBuilder<LabTests> filterBuilder;

        public MongoDBLabTestsModel(IMongoClient mongoClient)
        {
            filterBuilder = Builders<LabTests>.Filter;
            IMongoDatabase database = mongoClient.GetDatabase(DatabaseName);
            labtestsCollection = database.GetCollection<LabTests>(CollectionName);
        }

        //This method adds a new lab test result to a specific entity in the lab tests database
        public async Task<int> CreateTestAsync(LabTestResult labTest)
        {
            var filter = filterBuilder.Eq(l => l.id, labTest.patientId);
            LabTests newLabTest = await labtestsCollection.Find(filter).SingleOrDefaultAsync();
            newLabTest.AddTestResult(labTest);
            await labtestsCollection.ReplaceOneAsync(filter, newLabTest);
            return newLabTest.numberOfNegativeTests;
        }

        //This method creates a new LabTests entity for a Patient/Potential Patient and adds it to the lab tests
        //database. It is only activated on the first time that an entity is created.
        public async Task InitLabTestsAsync(string govtId)
        {
            LabTests newLabTest = new LabTests(govtId);
            await labtestsCollection.InsertOneAsync(newLabTest);
        }

        //This method finds and returns the person's labtset record by id
        public async Task<LabTests> GetLabTests(string id)
        {
            var filter = filterBuilder.Eq( l => l.id, id);
            return await labtestsCollection.Find(filter).SingleOrDefaultAsync();
        }

        //Reset current collection
        public async Task ResetCollectionAsync()
        {
            await labtestsCollection.DeleteManyAsync(filterBuilder.Empty);
        }
    }
}