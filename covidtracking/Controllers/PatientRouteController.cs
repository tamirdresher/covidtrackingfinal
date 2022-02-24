using covidtracking.Database;
using covidtracking.Dtos;
using Microsoft.AspNetCore.Mvc;
using covidtracking.Utilities;

namespace covidtracking.Controllers{
    [ApiController]
    [Route("[controller]")]
    public class PatientRouteController : ControllerBase{
        private readonly IPatientsDB _patientsDbModel;
        private readonly IPatientRoutesDB _model;

        public PatientRouteController(IPatientRoutesDB model, IPatientsDB patientsDbModel, IIsolatedDB isolatedDbModel)
        {
            _model = model;
            _patientsDbModel = patientsDbModel;
        }

        //Returns the list of all the locations the patient visited in during the last 7 days
        //GET /patients/{id}/route
        [HttpGet]
        [Route("/patients/{id}/route")]
        public async Task<ActionResult<IEnumerable<PatientRouteDto>>> GetPatientRouteAsync([FromRoute]string id){
            var patientRoute = await _model.GetPatientRouteByIdAsync(id);
            if(patientRoute is null){
                return NotFound();
            }
            PatientRouteDto patientRouteDto = patientRoute.PatientRouteAsDto();
            return Ok(patientRouteDto);
        }
    }
}