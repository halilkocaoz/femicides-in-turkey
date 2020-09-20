using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Femicides.API.Controllers
{
    public class VictimController : ApiController
    {
        public async Task<IActionResult> GetAllByFilters([FromQuery]string exampleFilter)
        {
            object p = Context.Victim.ToList();
            return Succes("Every morning around nine", p);
        }

        [HttpGet("{values}")]
        public async Task<IActionResult> GetMultipleByIds([FromRoute]string values)
        {
            return Succes("You wait patiently in line");
        }

        [HttpGet("{value:int}")]
        public async Task<IActionResult> GetSingleById([FromRoute]int value)
        {
            return Succes("I will make you coffee with extra cream");
        }
    }
}