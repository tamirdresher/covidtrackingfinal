using covidtracking.Database;
using covidtracking.Dtos;
using covidtracking.Entities;
using Microsoft.AspNetCore.Mvc;
using covidtracking.Utilities;
using System.Collections;

namespace covidtracking.Controllers{
    [ApiController]
    [Route("[controller]")]
    public class PatientEncountersController : ControllerBase{
        private readonly IPatientsDB _patientsDbModel;
        private readonly IPotentialPatientsDB _potentialPatientsDbModel;
        private readonly IIsolatedDB _isolatedDbModel;
        private readonly ILabTestsDB _labTestsModel;
        private readonly IPatientsEncountersDB _model;
        private readonly IStatisticsDb _statisticsDbModel;

        public PatientEncountersController(IPatientsEncountersDB model, IPatientsDB patientsDbModel, IPotentialPatientsDB potentialPatientsDbModel, IIsolatedDB isolatedDbModel,
         ILabTestsDB labTestsModel, IStatisticsDb statisticsDbModel)
        {
            _model = model;
            _potentialPatientsDbModel = potentialPatientsDbModel;
            _isolatedDbModel = isolatedDbModel;
            _patientsDbModel = patientsDbModel;
            _labTestsModel = labTestsModel;
            _statisticsDbModel = statisticsDbModel;
        }

        /* GET */

        //Return the list of the people the patient met during the last 7 days
        //GET /patients/{id}/encounters
        [HttpGet]
        [Route("/patients/{id}/encounters")]
        public async Task<ActionResult<PatientEncountersDto>> GetEncountersAsync([FromRoute]string id){
            var patient = await _model.GetPatientEncountersAsync(id);
            if(patient is null){
                return NotFound();
            }
            return Ok(patient.PatientEncountersAsDto());
        }
        
        //Returns the list of encounters where the person details were not inserted yet
        //GET /patients/potential
        [HttpGet]
        [Route("/patients/potential")]
        public ActionResult<IEnumerable<PotentialPatientsEncounterDto>> GetPotentialPatientsAsync(){
            HashSet<string> potentialPatients = _potentialPatientsDbModel.CreateHashSet();
            Hashtable patientsTable = _patientsDbModel.CreateHashTable().Result;
            var encounters = _model.GetPotentialPatientsEncounters(potentialPatients, patientsTable);
            if(encounters == null)
                return NotFound();
            return Ok(encounters);
        }

        /* PUT */

        //Add the details of a person the patient met during the last 7 days
        //PUT /patients/{id}/encounters
        [HttpPut]
        [Route("/patients/{id}/encounters")]
        public async Task<ActionResult> AddEncounteredAsync([FromRoute]string id, [FromBody] CreatePotentialPatientDto createPotentialPatientDto){
            var patientEncounters = await _model.GetPatientEncountersAsync(id);
            if(patientEncounters is null){
                return NotFound();
            }
            PotentialPatient potentialPatient = new PotentialPatient(createPotentialPatientDto);
            if(_potentialPatientsDbModel.CheckValidPotentialPatientInput(potentialPatient) == false){
                return BadRequest();
            }
            if(_potentialPatientsDbModel.CheckIfPotentialPatientExistsAsync(potentialPatient.key).Result == false){
                    //Update isolated DB
                    await _isolatedDbModel.CreateIsolated(potentialPatient.key, id);
                    //Add to suspects DB
                    await _potentialPatientsDbModel.CreatePotentialPatientAsync(potentialPatient);
                    //Update statistics DB
                    _statisticsDbModel.UpdateIsolated('+');
                }
            //Add to patient's list
            await _model.AddPatientEncounterAsync(id, potentialPatient);
            //Return object
            var PotentialPatientsEncounterDto = new PotentialPatientsEncounterDto(
                _patientsDbModel.GetPatientAsync(id).Result, potentialPatient
            );
            return Ok(PotentialPatientsEncounterDto);
        }
    }
}