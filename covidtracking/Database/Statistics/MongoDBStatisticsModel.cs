using covidtracking.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace covidtracking.Database{
    public class MongoDBStatisticsModel : IStatisticsDb
    {
        //Database access constants
        private const string DatabaseName = "covidtracking";
        private const string CollectionName = "statistics";

        //Properties
        private readonly IMongoCollection<Statistics> statisticsCollection;
        private readonly FilterDefinitionBuilder<Statistics> filterBuilder;

        public MongoDBStatisticsModel(IMongoClient mongoClient)
        {
            filterBuilder = Builders<Statistics>.Filter;
            IMongoDatabase database = mongoClient.GetDatabase(DatabaseName);
            statisticsCollection = database.GetCollection<Statistics>(CollectionName);
        }


        //This method checks if a city is already included in the database (from past patients).
        //If not - the city will be added and return true.
        //If the city exists - return false.
        public bool AddCityToDb(string city)
        {
            bool addCity = false;
            Statistics statistics = statisticsCollection.Find(new BsonDocument()).FirstOrDefault();
            if(statistics == null){
                statistics = new Statistics();
            }
            foreach(CityStatistics cs in statistics.cityStatistics){
                if(city == cs.city){
                    addCity = true;
                    break;
                }
            }
            if(addCity == false){
                statistics.cityStatistics.Add(new CityStatistics(city));
                UpdateStatistics(statistics);
            }
            return addCity;
        }

        public Statistics GetCurrentStatistics()
        {
            return statisticsCollection.Find(new BsonDocument()).FirstOrDefault();
        }
        
        //This method resetes the healed collection and creates a blank new one.
        public void InitStatisticsDb()
        {
            statisticsCollection.DeleteMany(filterBuilder.Empty);
            statisticsCollection.InsertOne(new Statistics());
        }

        public async Task ResetCollectionAsync()
        {
            await statisticsCollection.DeleteManyAsync(filterBuilder.Empty);
        }

        public void UpdateCityInfected(string city, char operation)
        {
            if(operation != '+' && operation != '-')
                return;
            Statistics statistics = statisticsCollection.Find(new BsonDocument()).FirstOrDefault();
            if(statistics == null){
                statistics = new Statistics();
            }
            foreach(CityStatistics cs in statistics.cityStatistics){
                if(city == cs.city){
                    if(operation == '+'){
                        cs.infected+=1;
                        statistics.infected+=1;
                    }   
                    else{
                        cs.infected-=1;
                        statistics.infected-=1;
                    }
                }
            }
            UpdateStatistics(statistics);
        }

        public void UpdateHealed()
        {
            Statistics statistics = statisticsCollection.Find(new BsonDocument()).FirstOrDefault();
            if(statistics == null){
                statistics = new Statistics();
            }
            statistics.healed+=1;
            UpdateStatistics(statistics);
        }
        

        public void UpdateIsolated(char operation)
        {
            if(operation != '+' && operation != '-')
                return;
            Statistics statistics = statisticsCollection.Find(new BsonDocument()).FirstOrDefault();
            if(statistics == null){
                statistics = new Statistics();
            }
            if(operation == '+'){
                statistics.isolated+=1;
            }   
            else{
                statistics.isolated-=1;
            }
            UpdateStatistics(statistics);
        }

        public void UpdateStatistics(Statistics newStats)
        {
            statisticsCollection.DeleteMany(filterBuilder.Empty);
            statisticsCollection.InsertOne(newStats);
        }
    }
}