using covidtracking.Database;
using covidtracking.Dtos;
using covidtracking.Entities;
using Microsoft.AspNetCore.Mvc;
using covidtracking.Utilities;

namespace covidtracking.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientsDB _model;
        private readonly IPotentialPatientsDB _potentialPatientsDbModel;
        private readonly IIsolatedDB _isolatedDbModel;
        private readonly IPatientRoutesDB _routesDbModel;
        private readonly ILabTestsDB _labtestsDbModel;
        private readonly IPatientsEncountersDB _patientEncountersDbModel;
        private readonly IStatisticsDb _statisticsDbModel;
        private readonly IInfectedDB _infectedDbModel;
        public PatientsController(IPatientsDB model, IPotentialPatientsDB potentialPatientsDbModel,
                                    IIsolatedDB isolatedDbModel, IPatientRoutesDB routesDbModel,
                                    ILabTestsDB labtestsDbModel, IPatientsEncountersDB patientEncountersDbModel,
                                    IStatisticsDb statisticsDbModel, IInfectedDB infectedDbModel)
        {
            _model = model;
            _potentialPatientsDbModel = potentialPatientsDbModel;
            _isolatedDbModel = isolatedDbModel;
            _routesDbModel = routesDbModel;
            _labtestsDbModel = labtestsDbModel;
            _patientEncountersDbModel = patientEncountersDbModel;
            _statisticsDbModel = statisticsDbModel;
            _infectedDbModel = infectedDbModel;
        }

        /* GET */

        //This method returns all the verified patients from the patients DB
        //GET /patients
        [HttpGet]
        [Route("/patients")]
        public async Task<ActionResult<List<GetPatientDto>>> GetPatientsAsync()
        {
            var patients = (await _model.GetPatientsAsync());
            List<GetPatientDto> list = new List<GetPatientDto>();
            foreach (Patient p in patients)
            {
                list.Add(new GetPatientDto(p));
            }
            return Ok(list);
        }

        //Returns the person details and whether he is sick or not together with all his/her lab tests
        //GET /patients/{id}/full
        [HttpGet]
        [Route("/patients/{id}/full")]
        public async Task<ActionResult<PatientInformationDto>> GetPatientAsync([FromRoute] string id)
        {
            var patient = await _model.GetPatientAsync(id);
            var labtests = await _labtestsDbModel.GetLabTests(id);
            if (patient is null)
            {
                return NotFound();
            }
            PatientInformationDto patientInformationDto = new PatientInformationDto(patient, labtests);
            return Ok(patientInformationDto);
        }

        /* PUT */

        //This method creates a new Patient entity.
        //After creating the Patient object it adds the Patient to the patients DB, the isolated DB and initializes a new entity for
        //the patient in the patients route DB.
        //PUT /patients
        [HttpPut]
        [Route("/patients")]
        public async Task<ActionResult<string>> CreatePatientAsync([FromBody] CreatePatientDto createPatientDto)
        {//ADD REGEX FOR ID, EMAIL, PHONE etc..
            Patient patient = new Patient(createPatientDto);
            if (_model.CheckValidPatientInput(patient) == false)
            {
                return BadRequest();
            }
            await _model.CreatePatientAsync(patient);
            await _isolatedDbModel.CreateIsolated(patient.govtId, "");
            await _labtestsDbModel.InitLabTestsAsync(patient.govtId);
            await _routesDbModel.InitPatientRoute(patient.govtId);
            await _patientEncountersDbModel.InitPatientEncountersAsync(patient.govtId);
            await _infectedDbModel.AddInfectedToDB(patient.govtId, DateTime.UtcNow);
            _statisticsDbModel.AddCityToDb(patient.address.city);
            _statisticsDbModel.UpdateIsolated('+');
            if (patient.isCovidPositive == true)
            {
                _statisticsDbModel.UpdateCityInfected(patient.address.city, '+');
            }
            return Ok(patient.govtId);
        }

        //This method adds a new Visit to a Patient's route.
        //The Visit is updated in the patients route DB
        //PUT /patients/{id}/route
        [HttpPut]
        [Route("/patients/{id}/route")]
        public async Task<ActionResult<PatientVisitDto>> AddVisitToPatientAsync([FromRoute] string id, [FromBody] CreateVisitDto createVisitDto)
        {
            var patient = await _model.GetPatientAsync(id);
            if (patient is null)
            {
                return NotFound();
            }
            Visit visit = new Visit(createVisitDto);
            if (_routesDbModel.CheckValidVisitInput(visit) == false || (DateTime.Now - createVisitDto.dateOfVisit).TotalDays > 7)
            {
                return BadRequest();
            }
            await _routesDbModel.CreateVisitAsync(id, visit);
            _statisticsDbModel.AddCityToDb(visit.siteAddress.city);
            PatientVisitDto patientVisitDto = new PatientVisitDto(visit, id);
            return Ok(patientVisitDto);
        }

        /* POST */

        //This endpoint transforms the potential patient to an actual patient.
        //This method activates all the needed creation functions to initialize all the entity's
        //information across the different databases.
        // //POST patients/potential/{potentialPatientId}
        [HttpPost]
        [Route("/patients/potential/{potentialPatientId}")]
        public async Task<ActionResult<PatientDto>> MakeInterviewAsync([FromRoute] string potentialPatientId, [FromBody] CreatePatientDto potentialToPatientDto)
        {
            var potentialPatient = await _potentialPatientsDbModel.GetPotentialPatientByIdAsync(potentialPatientId);
            if (potentialPatient is null)
            {
                return NotFound();
            }
            string infectedBy = await _isolatedDbModel.GetEncounteredPatientId(potentialPatientId);
            Patient patient = (new Adapters()).PotentialPatientToPatient(potentialToPatientDto, infectedBy);
            if (_model.CheckValidPatientInput(patient) == false)
            {
                return BadRequest();
            }
            await _model.CreatePatientAsync(patient);
            await _labtestsDbModel.InitLabTestsAsync(patient.govtId);
            await _routesDbModel.InitPatientRoute(patient.govtId);
            await _patientEncountersDbModel.InitPatientEncountersAsync(patient.govtId);
            await _isolatedDbModel.UpdateIsolatedEntityAsync(potentialPatientId, patient.govtId);
            await _potentialPatientsDbModel.DeletePotentialPatient(potentialPatientId);
            _statisticsDbModel.AddCityToDb(patient.address.city);
            if (patient.isCovidPositive == true)
            {
                _statisticsDbModel.UpdateCityInfected(patient.address.city, '+');
            }
            return Ok(patient.PatientAsGetPatientDto());
        }

        /* DELETE */

        //This endpoint resets the database
        //DELETE patients/all
        [HttpDelete]
        [Route("/patients/all")]
        public async Task ResetDatabasteAsync()
        {
            await _model.ResetCollectionAsync();
            await _isolatedDbModel.ResetCollectionAsync();
            await _labtestsDbModel.ResetCollectionAsync();
            await _patientEncountersDbModel.ResetCollectionAsync();
            await _potentialPatientsDbModel.ResetCollectionAsync();
            await _routesDbModel.ResetCollectionAsync();
            await _statisticsDbModel.ResetCollectionAsync();
        }
    }
}