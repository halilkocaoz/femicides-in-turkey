using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Femicides.API.Controllers
{
    public class CityController : ApiController
    {
        public async Task<IActionResult> GetAllByFilters([FromQuery]string exampleFilter)
        {
            return Succes("Every morning around nine");
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