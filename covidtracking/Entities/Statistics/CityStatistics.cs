namespace covidtracking.Entities
{
    public class CityStatistics
    {
        public string city { get; set; }
        public int infected { get; set; }
        public CityStatistics(string city)
        {
            this.city = city;
            infected = 0;
        }
    }
}