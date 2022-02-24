namespace covidtracking.Dtos
{
    public class CreateLabTestDto
    {
        public string labId { get; set; }
        public string testId { get; set; }
        public string patientId { get; set; }
        public DateTime testDate { get; set; }
        public bool isCovidPositive { get; set; }
    }
}