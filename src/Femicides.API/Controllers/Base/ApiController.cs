using Femicides.API.Extensions;
using Femicides.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Femicides.API.Controllers
{
    [Route("api/[controller]")]
    public class ApiController : ControllerBase
    {
        public static readonly sbyte maxDataCountPerPage = 10;

        public FemicidesContext Context => (FemicidesContext)HttpContext?.RequestServices.GetService(typeof(FemicidesContext));
        public MemoryCacheEntryOptions MemoryCacheExpOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = System.DateTime.Now.AddDays(1),
            Priority = CacheItemPriority.High
        };

        [NonAction]
        public object Pagination(int count, int selectedPage, string queryString)
        {
            var totalPages = CalculateTotalPage(count);
            var requestHostPath = "https://" + Request.Host + Request.Path;

            var info = new
            {
                count,
                pages = totalPages,
                next = requestHostPath.NextPage(totalPages,selectedPage,queryString),
                prev = requestHostPath.PrevPage(totalPages,selectedPage,queryString)
            };

            return info;
        }

        [NonAction]
        private int CalculateTotalPage(int dataCount) => dataCount % maxDataCountPerPage != 0 ? dataCount / maxDataCountPerPage + 1 : dataCount / maxDataCountPerPage;

        [NonAction]
        public IActionResult Succes(string message = null, object data = null, object info = null)
        {
            var rm = new ReturnModel
            {
                Message = message,
                Information = info,
                Data = data,
            };

            return Ok(rm);
        }

        [NonAction]
        public IActionResult Error(int code = 400, string message = null, object data = null, object info = null)
        {
            var rm = new ReturnModel
            {
                Message = message,
                Information = info,
                Data = data,
            };
            if(code == 404)
            {
                if (rm.Message == null)
                {
                    rm.Message = "There's nothing here";
                }

                return NotFound(rm);
            }

            return BadRequest(rm);
        }
    }
}