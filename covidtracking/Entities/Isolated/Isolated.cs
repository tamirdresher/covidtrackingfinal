using System.ComponentModel.DataAnnotations;

namespace covidtracking.Entities
{
    public class Isolated
    {
        public string id { get; set; }
        public string encounteredId { get; set; }
    }
}