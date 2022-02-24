namespace covidtracking.Entities
{
    public class PatientEncounter
    {
        public string id { get; init; }
        public List<PotentialPatient> potentialPatientsEncountered { get; set; }
        public PatientEncounter(string id)
        {
            this.id = id;
            potentialPatientsEncountered = new List<PotentialPatient>();
        }
    }
}