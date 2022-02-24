using System.Text.RegularExpressions;
using covidtracking.Entities;
using MongoDB.Driver;

namespace covidtracking.Database
{
    public class MongoDBPatientsRoutesModel : IPatientRoutesDB
    {
        //Database access constants
        private const string DatabaseName = "covidtracking";
        private const string CollectionName = "patientsroutes";

        //Properties
        private readonly IMongoCollection<PatientRoute> routesCollection;
        private readonly FilterDefinitionBuilder<PatientRoute> filterBuilder;

        public MongoDBPatientsRoutesModel(IMongoClient mongoClient)
        {
            filterBuilder = Builders<PatientRoute>.Filter;
            IMongoDatabase database = mongoClient.GetDatabase(DatabaseName);
            routesCollection = database.GetCollection<PatientRoute>(CollectionName);
        }

        //This method checks if a new Visit object addition attempt is valid by verifying the input format.
        //The method returns true if the format is valid or false if not.
        public bool CheckValidVisitInput(Visit visit)
        {
            var cityRegex = @"^[a-zA-Z]+(([ ][a-zA-Z ])?[a-zA-Z]*)*$";
            if (Regex.Match(visit.siteAddress.city, cityRegex).Success == false ||
                visit.siteAddress.street == "")
                return false;
            return true;
        }

        //This method finds the PatientRoute object containing the person's route by using a filter
        //by id, than appends the new visit to the route's List and updates the DB with the new object.
        public async Task CreateVisitAsync(string id, Visit visit)
        {
            var filter = filterBuilder.Eq(p => p.id, id);
            PatientRoute updatedPatientRoute = (await routesCollection.Find(filter).SingleOrDefaultAsync());
            updatedPatientRoute.route.Add(visit);
            routesCollection.ReplaceOne(filter, updatedPatientRoute);
        }

        //This method finds and returns the route of a patient by using a filter by id.
        public async Task<PatientRoute> GetPatientRouteByIdAsync(string id)
        {
            var filter = filterBuilder.Eq(p => p.id, id);
            return await routesCollection.Find(filter).SingleOrDefaultAsync();
            //CHANGE TO PATIENTROUTEDTO?
        }

        //This method initialize a PatientRoute object for a new created Patient.
        //Sets the PatientRoute id as the Patient's id to be used as a link.
        public async Task InitPatientRoute(string id)
        {
            //add check if already exists
            await routesCollection.InsertOneAsync(new PatientRoute(id));
        }

        //Reset current collection
        public async Task ResetCollectionAsync()
        {
            await routesCollection.DeleteManyAsync(filterBuilder.Empty);
        }
    }
}