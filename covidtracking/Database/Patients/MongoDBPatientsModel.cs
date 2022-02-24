using System.Text.RegularExpressions;
using System.Collections;
using covidtracking.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace covidtracking.Database
{
    public class MongoDBPatientsModel : IPatientsDB
    {
        //Constants
        private const string DatabaseName = "covidtracking";
        private const string CollectionName = "patients";
        //Properties
        private readonly IMongoCollection<Patient> patientsCollection;
        private readonly FilterDefinitionBuilder<Patient> filterBuilder;

        public MongoDBPatientsModel(IMongoClient mongoClient)
        {
            filterBuilder = Builders<Patient>.Filter;
            IMongoDatabase database = mongoClient.GetDatabase(DatabaseName);
            patientsCollection = database.GetCollection<Patient>(CollectionName);
        }

        //This method finds and returns a Patient by id
        public async Task<Patient> GetPatientAsync(string id)
        {
            var filter = filterBuilder.Eq(p => p.govtId, id);
            return await patientsCollection.Find(filter).SingleOrDefaultAsync();
        }

        //This method finds and returns a list of all the patients currently in the database.
        public async Task<List<Patient>> GetPatientsAsync()
        {
            return await patientsCollection.Find(new BsonDocument()).ToListAsync();
        }

        //This method creates and appends a new Patient entity to the db if he doesn't already exist
        public async Task CreatePatientAsync(Patient patient)
        {
            var filter = filterBuilder.Eq(p => p.govtId, patient.govtId);
            if (await patientsCollection.Find(filter).SingleOrDefaultAsync() == null)
                await patientsCollection.InsertOneAsync(patient);
        }

        //This method is used to delete a Patient entity from the database
        public async Task DeletePatientAsync(string id)
        {
            var filter = filterBuilder.Eq(p => p.govtId, id);
            await patientsCollection.DeleteOneAsync(filter);
        }

        public async Task MakeSickAsync(Patient patient)
        {
            var filter = filterBuilder.Eq(p => p.govtId, patient.govtId);
            patient.isCovidPositive = true;
            await patientsCollection.ReplaceOneAsync(filter, patient);
        }

        //Reset current collection
        public async Task ResetCollectionAsync()
        {
            await patientsCollection.DeleteManyAsync(filterBuilder.Empty);
        }

        public async Task<Hashtable> CreateHashTable()
        {
            Hashtable patientsTable = new Hashtable();
            var patients = await patientsCollection.Find(new BsonDocument()).ToListAsync();
            foreach (Patient p in patients)
            {
                patientsTable.Add(p.govtId, p);
            }
            return patientsTable;
        }

        //This method checks if a new Patient object addition attempt is valid by verifying the input format.
        //The method returns true if the format is valid or false if not.
        public bool CheckValidPatientInput(Patient patient)
        {
            var filter = filterBuilder.Eq(p => p.govtId, patient.govtId);
            var idRegex = @"^(\d{9})$";
            var nameRegex = @"^[a-zA-Z]+(([ -][a-zA-Z ])?[a-zA-Z]*)*$";
            var phoneRegex = @"^[0-9.-]+$";
            var cityRegex = @"^[a-zA-Z]+(([ ][a-zA-Z ])?[a-zA-Z]*)*$";
            if (patient == null || patientsCollection.Find(filter).SingleOrDefault() != null ||
                Regex.Match(patient.govtId, idRegex).Success == false ||
                Regex.Match(patient.firstName, nameRegex).Success == false ||
                Regex.Match(patient.lastName, nameRegex).Success == false ||
                Regex.Match(patient.phoneNumber, phoneRegex).Success == false ||
                Regex.Match(patient.address.city, cityRegex).Success == false ||
                (patient.infectedByPatientID != null && Regex.Match(patient.infectedByPatientID, idRegex).Success == false))
                return false;
            return true;
        }

        //This method filters the Patients in the patients DB and returns a list of all the patients
        //whose id exists in the given HashSet.
        //Used in the get all infected since endpoint.
        public async Task<List<Patient>> GetPatientsWithIdsAsync(HashSet<string> infectedIds)
        {
            var patients = await patientsCollection.Find(new BsonDocument()).ToListAsync();
            List<Patient> matchingPatients = new List<Patient>();
            foreach (Patient p in patients)
            {
                if (infectedIds.Contains(p.govtId) == true)
                {
                    matchingPatients.Add(p);
                }
            }
            return matchingPatients;
        }
    }
}