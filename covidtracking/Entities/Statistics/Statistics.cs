namespace covidtracking.Entities{
    public class Statistics{
        public int infected { get; set; }
        public int healed { get; set; }
        public int isolated { get; set; }
        public List<CityStatistics> cityStatistics { get; set; }

        public Statistics() 
        {
            infected = 0;
            healed = 0;
            isolated = 0;
            cityStatistics = new List<CityStatistics>();
        }
    }
}