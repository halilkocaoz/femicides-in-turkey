using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Femicides.API.Controllers
{
    public class StatisticController : ApiController
    {
        public IActionResult Index()
        {
            var urls = new {
                killer = "https://femicidesinturkey.com/api/statistic/killer",
                method = "https://femicidesinturkey.com/api/statistic/method",
                cause = "https://femicidesinturkey.com/api/statistic/cause",
            };
            return Succes(null, null, urls);
        }

        [HttpGet("killer")]
        public IActionResult Killer()
        {
            var description = new
            {
                count = "How many women killed by whom the person definition?",
                filterUrl = "It helps to search victims with the killer definition filter.",
            };

            var Killer = Context.Perpetrator
                .GroupBy(g => g.Definition)
                .Select(s => new { name = s.Key, count = s.Count(), filterUrl = "https://femicidesinturkey.com/api/victim/?killer=" + s.Key.Replace(" ", "%20") })
                .OrderByDescending(x => x.count);

            return Succes(null, Killer, description);
        }

        [HttpGet("method")]
        public IActionResult Method()
        {
            var description = new
            {
                count = "How many women killed with this method?",
                filterUrl = "It helps to search victims with the method filter.",
            };

            var Method = Context.VictimMethodsOfKilled
                .GroupBy(g => g.Method)
                .Select(s => new { name = s.Key, count = s.Count(), filterUrl = "https://femicidesinturkey.com/api/victim/?method=" + s.Key.Replace(" ", "%20") })
                .OrderByDescending(x => x.count);

            return Succes(null, Method, description);
        }

        [HttpGet("cause")]
        public IActionResult Cause()
        {
            var description = new
            {
                count = "How many women killed with this cause?",
                filterUrl = "It helps to search victims with the cause filter.",
            };

            var Causes = Context.VictimCausesOfKilled
                .GroupBy(p => p.Cause)
                .Select(s => new { name = s.Key, count = s.Count(), filterUrl = "https://femicidesinturkey.com/api/victim?cause=" + s.Key.Replace(" ", "%20") })
                .OrderByDescending(x => x.count);

            return Succes(null, Causes, description);
        }
    }
}