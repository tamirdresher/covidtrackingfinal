using covidtracking.Dtos;

namespace covidtracking.Entities
{
    public class Infected
    {
        public string id { get; set; }
        public DateTime infectedDateTime { get; set; }

        public Infected(CreateinfectedDto createinfectedDto)
        {
            id = createinfectedDto.id;
            infectedDateTime = createinfectedDto.infectedDateTime;
        }

        public Infected(string id, DateTime infectedDateTime)
        {
            this.id = id;
            this.infectedDateTime = infectedDateTime;
        }
    }
}