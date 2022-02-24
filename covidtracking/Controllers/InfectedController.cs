using covidtracking.Database;
using covidtracking.Dtos;
using covidtracking.Entities;
using Microsoft.AspNetCore.Mvc;
using covidtracking.Utilities;

namespace covidtracking.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InfectedController : ControllerBase
    {
        private readonly IInfectedDB _model;
        private readonly IPatientsDB _patientsDbModel;
        public InfectedController(IInfectedDB model, IPatientsDB patientsDbModel)
        {
            _model = model;
            _patientsDbModel = patientsDbModel;
        }

        //Will display a list of all sick people who were added after the value of 'since'
        //GET /patients/new?since=[VALUE]
        [HttpGet]
        [Route("/patients/new")]
        public async Task<ActionResult<IEnumerable<GetInfectedDto>>> GetPatientsSince([FromQuery] string since)
        {
            DateTime sinceDate;
            try
            {
                sinceDate = DateTime.Parse(since);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
            HashSet<string> infectedIds = _model.GetInfectedSince(sinceDate);
            var infectedPatients = await _patientsDbModel.GetPatientsWithIdsAsync(infectedIds);
            List<GetInfectedDto> infectedSince = new List<GetInfectedDto>();
            foreach (Patient p in infectedPatients)
            {
                GetInfectedDto newInfected;
                if (p.infectedByPatientID == null)
                {
                    newInfected = new GetInfectedDto(
                        null, (new Adapters()).PotentialPatientToPatient(p)
                );
                }
                else
                {
                    newInfected = new GetInfectedDto(
                        await _patientsDbModel.GetPatientAsync(p.infectedByPatientID),
                        (new Adapters()).PotentialPatientToPatient(p)
                    );
                }
                infectedSince.Add(newInfected);
            }
            if (infectedSince.Count() == 0)
                return NotFound();
            return Ok(infectedSince);
        }
    }
}