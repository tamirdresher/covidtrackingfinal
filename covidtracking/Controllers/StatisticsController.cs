using covidtracking.Database;
using covidtracking.Entities;
using Microsoft.AspNetCore.Mvc;

namespace covidtracking.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsDb _model;

        public StatisticsController(IStatisticsDb model)
        {
            _model = model;
        }

        //Returns statistics about the current state – amount of sicks, amount of isolated,
        // how many have healed, and how many sick we have per city
        //GET /statistics
        [HttpGet]
        [Route("/statistics")]
        public ActionResult<Statistics> GetCurrentStatistics()
        {
            var statistics = _model.GetCurrentStatistics();
            if (statistics == null)
            {
                _model.InitStatisticsDb();
                return Ok(new Statistics());
            }
            return Ok(statistics);
        }
    }
}