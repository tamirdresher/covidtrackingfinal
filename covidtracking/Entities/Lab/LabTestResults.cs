using covidtracking.Dtos;

namespace covidtracking.Entities
{
    public class LabTestResult
    {
        public string labId { get; set; }
        public string testId { get; set; }
        public string patientId { get; set; }
        public DateTime testDate { get; set; }
        public bool isCovidPositive { get; set; }
        public LabTestResult(CreateLabTestDto createLabTestDto)
        {
            labId = createLabTestDto.labId;
            testId = createLabTestDto.testId;
            patientId = createLabTestDto.patientId;
            testDate = createLabTestDto.testDate;
            isCovidPositive = createLabTestDto.isCovidPositive;
        }

        public LabTestResult(string labId, string testId, string patientId, DateTime testDate, bool isCovidPositive)
        {
            this.labId = labId;
            this.testId = testId;
            this.patientId = patientId;
            this.testDate = testDate;
            this.isCovidPositive = isCovidPositive;
        }
    }
}