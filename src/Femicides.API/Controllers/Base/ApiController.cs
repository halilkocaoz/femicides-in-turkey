using Femicides.API.Extensions;
using Femicides.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Femicides.API.Controllers
{
    [Route("api/[controller]")]
    public class ApiController : ControllerBase
    {
        public static readonly sbyte maxDataCountPerPage = 10;
        public FemicidesContext Context => (FemicidesContext)HttpContext?.RequestServices.GetService(typeof(FemicidesContext));

        [NonAction]
        public object Information(int count, int selectedPage, KeyValuePair<string, StringValues>[] queries)
        {
            var queryString = queries.ToStringWithOutPageParam();
            var totalPages = count % maxDataCountPerPage != 0 ? count / maxDataCountPerPage + 1 : count / maxDataCountPerPage;
            var requestHostPath = "https://" + Request.Host + Request.Path;

            var info = new
            {
                count,
                pages = totalPages,
                next = requestHostPath.RequestedUrlPaginationWithParams(totalPages,selectedPage,queryString,1),
                prev = requestHostPath.RequestedUrlPaginationWithParams(totalPages,selectedPage,queryString,-1)
            };
            return info;
        }

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