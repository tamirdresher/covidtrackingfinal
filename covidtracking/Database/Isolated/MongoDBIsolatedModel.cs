using covidtracking.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace covidtracking.Database{
    public class MongoDBIsolatedModel : IIsolatedDB
    {
        //Database access constants
        private const string DatabaseName = "covidtracking";
        private const string CollectionName = "isolated";
        
        //Properties
        private readonly IMongoCollection<Isolated> isolatedCollection;
        private readonly FilterDefinitionBuilder<Isolated> filterBuilder;

        public MongoDBIsolatedModel(IMongoClient mongoClient)
        {
            filterBuilder = Builders<Isolated>.Filter;
            IMongoDatabase database = mongoClient.GetDatabase(DatabaseName);
            isolatedCollection = database.GetCollection<Isolated>(CollectionName);
        }

        //This method checks if an Isolated entity exist for a give key(id/potential key) and if not,
        //Creates and returns the new created entry
        public async Task<Isolated> CreateIsolated(string key, string ecnountered)
        {//MAYBE CHANGE RETURN VALUE? NEED TO RETURN THE NEW ENTITY?
            var isolated = await GetIsolatedByKeyAsync(key);
            if (isolated != null)
            {
                return null;
            }
            Isolated newIsolated = new Isolated()
            {
                id = key,
                encounteredId = ecnountered
            };
            await isolatedCollection.InsertOneAsync(newIsolated);
            return newIsolated;
        }

        //This method deletes an Isolated entity from the isolated database.
        //It is activated when a patient/potential patient gets 2 negative test results and he can be free again.
        public async Task DeleteIsolatedAsync(string id)
        {
            var filter = filterBuilder.Eq(i => i.id, id);
            await isolatedCollection.DeleteOneAsync(filter);
        }

        //This method returns an IEnumerable<string> implemented as a List of the ids the isolated patients
        public async Task<List<string>> GetIsolatedIdsAsync()
        {
            var ids = await isolatedCollection.Find(new BsonDocument()).ToListAsync();
            List<string> list = new List<string>();
            foreach (Isolated i in ids)
            {
                list.Add(i.id);
            }
            return list.Count() > 0 ? list : null;
        }

        //This method returns an IEnumerable<string> implemented as a List of the ids of the patients
        //that the isolated person has ecnountered.
        public async Task<List<string>> GetIsolatedEncounteredIdsAsync()
        {
            var ids = await isolatedCollection.Find(new BsonDocument()).ToListAsync();
            List<string> list = new List<string>();
            foreach (Isolated i in ids)
            {
                list.Add(i.encounteredId);
            }
            return list;
        }

        //This method is used to find and return an Isolated entity by a given key
        public async Task<Isolated> GetIsolatedByKeyAsync(string key)
        {
            var filter = filterBuilder.Eq(i => i.id, key);
            return await isolatedCollection.Find(filter).SingleOrDefaultAsync();
        }

        //This method is used to update the isolated person's id.
        //It is activated when a potential patient has been interviewed and turns to a patient.
        public async Task UpdateIsolatedEntityAsync(string oldKey, string newKey)
        {
            var filter = filterBuilder.Eq(i => i.id, oldKey);
            Isolated isolated = await isolatedCollection.Find(filter).SingleOrDefaultAsync();
            isolated.id = newKey;
            await isolatedCollection.ReplaceOneAsync(filter, isolated);
        }
        
        //This method returns the patient id that a given potential patient has encountered.
        //It is used to convert from a potential patient to an actual patient
        public async Task<string> GetEncounteredPatientId(string id){
            var filter = filterBuilder.Eq(i => i.id, id);
            Isolated isolated =  await isolatedCollection.Find(filter).SingleOrDefaultAsync();
            if(isolated == null){
                return null;
            }
            return isolated.encounteredId;
        }

        //Reset current collection
        public async Task ResetCollectionAsync()
        {
            await isolatedCollection.DeleteManyAsync(filterBuilder.Empty);
        }
    }
}