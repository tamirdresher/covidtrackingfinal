
using System.Text.RegularExpressions;
using covidtracking.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace covidtracking.Database
{
    public class MongoDBPotentialPatientsModel : IPotentialPatientsDB
    {
        //Database access constants
        private const string DatabaseName = "covidtracking";
        private const string CollectionName = "potentialpatients";

        //Properties
        private readonly IMongoCollection<PotentialPatient> potentialPatientsCollection;
        private readonly FilterDefinitionBuilder<PotentialPatient> filterBuilder;

        public MongoDBPotentialPatientsModel(IMongoClient mongoClient)
        {
            filterBuilder = Builders<PotentialPatient>.Filter;
            IMongoDatabase database = mongoClient.GetDatabase(DatabaseName);
            potentialPatientsCollection = database.GetCollection<PotentialPatient>(CollectionName);
        }

        //This method checks if a PotentialPatient we try to add already exists and if not,
        //adds the new potential patient to the potential patients DB
        public async Task CreatePotentialPatientAsync(PotentialPatient potentialPatient)
        {
            var filter = filterBuilder.Eq(p => p.key, potentialPatient.key);
            if (await potentialPatientsCollection.Find(filter).SingleOrDefaultAsync() == null)
                await potentialPatientsCollection.InsertOneAsync(potentialPatient);
        }

        //This method checks if a protential patient already exist.
        //Returns true if the key is already registered or false if not.
        //Used to check if a new LabTests entity needs to be created when adding a new Patient
        public async Task<bool> CheckIfPotentialPatientExistsAsync(string key)
        {
            var filter = filterBuilder.Eq(p => p.key, key);
            return (await potentialPatientsCollection.Find(filter).SingleOrDefaultAsync() != null);
        }

        //This method removes a single entity from the potentialpatients DB.
        //Used when converting to a verified Patient or when healing.
        public async Task DeletePotentialPatient(string key)
        {
            var filter = filterBuilder.Eq(s => s.key, key);
            await potentialPatientsCollection.DeleteOneAsync(filter);
        }

        //This method checks if a given key belongs to a Potential Patient in the potential patients
        //db and returns the potential patient if found or null if not
        public async Task<PotentialPatient> GetPotentialPatientByIdAsync(string key)
        {
            var filter = filterBuilder.Eq(p => p.key, key);
            return await potentialPatientsCollection.Find(filter).SingleOrDefaultAsync();
        }

        //Reset current collection
        public async Task ResetCollectionAsync()
        {
            await potentialPatientsCollection.DeleteManyAsync(filterBuilder.Empty);
        }

        //This method creates a HashSet containing all the keys of potential (uninterviewed) patients.
        //It is used to retreive all the encounters patients had where the person they encountered is
        //not yet interviewed.
        public HashSet<string> CreateHashSet()
        {
            HashSet<string> potentialKeySet = new HashSet<string>();
            var potentialPatients = potentialPatientsCollection.Find(new BsonDocument()).ToListAsync().Result;
            foreach (PotentialPatient p in potentialPatients)
            {
                potentialKeySet.Add(p.key);
            }
            return potentialKeySet;
        }

        //This method checks if a new PotentialPatient object addition attempt is valid by verifying the input format.
        //The method returns true if the format is valid or false if not.
        public bool CheckValidPotentialPatientInput(PotentialPatient potentialPatient)
        {
            var nameRegex = @"^[a-zA-Z]+(([ -][a-zA-Z ])?[a-zA-Z]*)*$";
            var phoneRegex = @"^[0-9.-]+$";
            if (Regex.Match(potentialPatient.firstName, nameRegex).Success == false ||
                Regex.Match(potentialPatient.lastName, nameRegex).Success == false ||
                Regex.Match(potentialPatient.phoneNumber, phoneRegex).Success == false)
                return false;
            return true;
        }
    }
}