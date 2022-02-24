using covidtracking.Database;
using covidtracking.Dtos;
using covidtracking.Entities;
using Microsoft.AspNetCore.Mvc;

namespace covidtracking.Controllers{
    [ApiController]
    [Route("[controller]")]
    public class LabTestsController : ControllerBase{
        private readonly ILabTestsDB _model;
        private readonly IPotentialPatientsDB _potentialPatientsDbModel;
        private readonly IPatientsDB _patientsDbModel;
        private readonly IIsolatedDB _isolatedDbModel;
        private readonly IStatisticsDb _statisticsDbModel;
        private readonly IInfectedDB _infectedDbModel;

        public LabTestsController(ILabTestsDB model, IPotentialPatientsDB potentialPatientsDbModel,
         IPatientsDB patientsDbModel, IIsolatedDB isolatedDbModel, IStatisticsDb statisticsDbModel,
         IInfectedDB infectedDbModel)
        {
            _model = model;
            _potentialPatientsDbModel = potentialPatientsDbModel;
            _patientsDbModel = patientsDbModel;
            _isolatedDbModel = isolatedDbModel;
            _statisticsDbModel = statisticsDbModel;
            _infectedDbModel = infectedDbModel;
        }

        /* POST */
        
        //Add a lab test result 
        //POST /labtests
        [HttpPost]
        [Route("/labtests")]
        public async Task<ActionResult<string>> CreateTestAsync([FromBody] CreateLabTestDto createLabTestDto){
            //Search the Patients databases for the given id.
            //If returns null, return a BadRequest since the id is invalid.
            string id = createLabTestDto.patientId;
            Patient patientSearch = _patientsDbModel.GetPatientAsync(id).Result;
            if(patientSearch == null){
                return BadRequest("Id doesn't exist in our database");
            }
            //The id exists in the Patients database, create and add the test to the labtests db
            LabTestResult labTest = new LabTestResult(createLabTestDto);
            int negativeTests = await _model.CreateTestAsync(labTest);
            if(negativeTests==2){
                if(patientSearch.isCovidPositive == true){
                    patientSearch.isCovidPositive = false;
                    _statisticsDbModel.UpdateCityInfected(patientSearch.address.city, '-');
                    _statisticsDbModel.UpdateHealed();
                    await _infectedDbModel.RemoveInfectedFromDB(labTest.patientId);
                }
                await _isolatedDbModel.DeleteIsolatedAsync(id);       
                _statisticsDbModel.UpdateIsolated('-');
            }
            if(labTest.isCovidPositive == true && patientSearch.isCovidPositive == false){
                await _patientsDbModel.MakeSickAsync(patientSearch);
                _statisticsDbModel.UpdateCityInfected(patientSearch.address.city, '+');
                await _infectedDbModel.AddInfectedToDB(labTest.patientId, labTest.testDate); 
            }  
            return Ok(id);
        }
    }
}