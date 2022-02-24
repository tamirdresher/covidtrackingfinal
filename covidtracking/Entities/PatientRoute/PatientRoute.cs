namespace covidtracking.Entities{
    public class PatientRoute{
        public string id { get; init; }
        public List<Visit> route { get; set; }
        public PatientRoute(string id){
            this.id=id;
            route = new List<Visit>();
        }
    }
}