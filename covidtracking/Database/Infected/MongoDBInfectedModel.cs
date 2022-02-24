using covidtracking.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace covidtracking.Database{
    public class MongoDBInfectedModel : IInfectedDB
    {
        //Database access constants
        private const string DatabaseName = "covidtracking";
        private const string CollectionName = "infected";

        //Properties
        private readonly IMongoCollection<Infected> infectedCollection;
        private readonly FilterDefinitionBuilder<Infected> filterBuilder;

        public MongoDBInfectedModel(IMongoClient mongoClient)
        {
            filterBuilder = Builders<Infected>.Filter;
            IMongoDatabase database = mongoClient.GetDatabase(DatabaseName);
            infectedCollection = database.GetCollection<Infected>(CollectionName);
        }

        public async Task AddInfectedToDB(string id, DateTime infectedDateTime)
        {
            var filter = filterBuilder.Eq( i => i.id, id);
            Infected infected = await infectedCollection.Find(filter).SingleOrDefaultAsync();
            if(infected == null){
                infected = new Infected(id, infectedDateTime);
                await infectedCollection.InsertOneAsync(infected);
            }

        }

        //This method returns all the Infected ids of Patients that got infected after a give date.
        public HashSet<string> GetInfectedSince(DateTime since)
        {
            var infected = infectedCollection.Find(new BsonDocument()).ToList();
            HashSet<string> set = new HashSet<string>();
            foreach (Infected i in infected)
            {
                if(i.infectedDateTime > since)
                    set.Add(i.id);
            }
            return set;
        }

        public async Task RemoveInfectedFromDB(string id)
        {
            var filter = filterBuilder.Eq( i => i.id, id);
            await infectedCollection.DeleteOneAsync(filter);
        }
    }
}