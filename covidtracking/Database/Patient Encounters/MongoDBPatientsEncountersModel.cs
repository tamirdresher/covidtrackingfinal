using System.Collections;
using covidtracking.Dtos;
using covidtracking.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace covidtracking.Database
{
    public class MongoDBPatientsEncountersModel : IPatientsEncountersDB
    {
        //Database access constants
        private const string DatabaseName = "covidtracking";
        private const string CollectionName = "patientencounters";

        //Properties
        private readonly IMongoCollection<PatientEncounter> patientEncountersCollection;
        private readonly FilterDefinitionBuilder<PatientEncounter> filterBuilder;

        public MongoDBPatientsEncountersModel(IMongoClient mongoClient)
        {
            filterBuilder = Builders<PatientEncounter>.Filter;
            IMongoDatabase database = mongoClient.GetDatabase(DatabaseName);
            patientEncountersCollection = database.GetCollection<PatientEncounter>(CollectionName);
        }

        //This method finds a Patient's encounters list and adds a new encounter with a give Potential Patient
        //than updates the patient encounters DB
        public async Task AddPatientEncounterAsync(string id, PotentialPatient potentialPatient)
        {
            var filter = filterBuilder.Eq(p => p.id, id);
            PatientEncounter patientEncounters = await patientEncountersCollection.Find(filter).SingleOrDefaultAsync();
            if (patientEncounters == null)
            {
                return;//CHANGE TO RETURN BADREQUEST OR NOT EXIST OR SOMETHING
            }
            patientEncounters.potentialPatientsEncountered.Add(potentialPatient);
            await patientEncountersCollection.ReplaceOneAsync(filter, patientEncounters);
        }


        //This method finds and returns the ecnounters of a Patient by using a filter with a given Patient id
        public async Task<PatientEncounter> GetPatientEncountersAsync(string id)
        {
            var filter = filterBuilder.Eq(p => p.id, id);
            return await patientEncountersCollection.Find(filter).SingleOrDefaultAsync();
        }

        //This method finds and returns a collection of all the encounters where the encountered patient
        //has not been interviewed yet.
        public IEnumerable<PotentialPatientsEncounterDto> GetPotentialPatientsEncounters(HashSet<string> potentialPatientsKeys,
            Hashtable patients)
        {
            var allEncounters = patientEncountersCollection.Find(new BsonDocument()).ToListAsync().Result;
            List<PotentialPatientsEncounterDto> encountersWithUninterviewed = new List<PotentialPatientsEncounterDto>();
            foreach (PatientEncounter potentialEncounters in allEncounters)
            {
                foreach (PotentialPatient potentialPatient in potentialEncounters.potentialPatientsEncountered)
                {
                    if (potentialPatientsKeys.Contains(potentialPatient.key))
                    {
                        encountersWithUninterviewed.Add(new PotentialPatientsEncounterDto(
                            (Patient)patients[potentialEncounters.id], potentialPatient));
                    }
                }
            }
            return encountersWithUninterviewed.Count() > 0 ? encountersWithUninterviewed : null;
        }

        //This method initializes a new patients encounter entity in the patients ecnounters database
        //The method will be activated when creating a new Patient or converting
        public async Task InitPatientEncountersAsync(string id)
        {
            await patientEncountersCollection.InsertOneAsync(new PatientEncounter(id));
        }

        //Reset current collection
        public async Task ResetCollectionAsync()
        {
            await patientEncountersCollection.DeleteManyAsync(filterBuilder.Empty);
        }
    }
}