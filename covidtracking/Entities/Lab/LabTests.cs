namespace covidtracking.Entities{
    public class LabTests{
        public string id { get; set; }
        public int numberOfNegativeTests = 0;
        public List<LabTestResult> labTestResults = new List<LabTestResult>();

        public LabTests(string id){
            this.id = id;
        }
        
        public void AddTestResult(LabTestResult labTest)
        {
            labTestResults.Add(labTest);
            if(labTest.isCovidPositive == false)
                numberOfNegativeTests++ ;
        }

    }
}