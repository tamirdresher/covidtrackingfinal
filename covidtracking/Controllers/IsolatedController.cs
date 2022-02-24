using covidtracking.Database;
using covidtracking.Dtos;
using covidtracking.Entities;
using Microsoft.AspNetCore.Mvc;

namespace covidtracking.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IsolatedController : ControllerBase
    {
        private readonly IIsolatedDB _model;
        private readonly IPotentialPatientsDB _potentialPatientsDbModel;
        private readonly IPatientsDB _patientsDbModel;

        public IsolatedController(IIsolatedDB model, IPotentialPatientsDB potentialPatientsDbModel, IPatientsDB patientsDbModel)
        {
            _model = model;
            _potentialPatientsDbModel = potentialPatientsDbModel;
            _patientsDbModel = patientsDbModel;
        }

        //Returns the list of all the people in the system that are in isolation 
        //(person is isolated until he has two negative tests since he encountered an infected person or reported infected)
        //GET /patients/isolated
        [HttpGet]
        [Route("/patients/isolated")]
        public async Task<ActionResult<IEnumerable<IsolatedDto>>> GetIsolatedAsync()
        {
            var isolated = await _model.GetIsolatedIdsAsync();
            if (isolated == null)
            {
                return NotFound();
            }
            var encountered = await _model.GetIsolatedEncounteredIdsAsync();
            List<IsolatedDto> isolatedList = new List<IsolatedDto>();
            foreach ((string id, string eId) in isolated.Zip(encountered))
            {
                var encounteredPatient = await (_patientsDbModel.GetPatientAsync(eId));
                IPatient potentialPatientDetails = await (_potentialPatientsDbModel.GetPotentialPatientByIdAsync(id)) == null ?
                    await (_patientsDbModel.GetPatientAsync(id)) : await (_potentialPatientsDbModel.GetPotentialPatientByIdAsync(id));
                isolatedList.Add(new IsolatedDto(encounteredPatient, potentialPatientDetails));
            }
            return Ok("isolatedList");
        }

        //DELETE /patients/isolated/{key}
        [HttpDelete]
        [Route("/patients/isolated/{key}")]
        public async Task DeleteIsolatedAsync([FromRoute] string key)
        {
            await _model.DeleteIsolatedAsync(key);
        }
    }
}