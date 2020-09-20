using Microsoft.AspNetCore.Mvc;

namespace Femicides.API.Controllers
{
    public class ApiController : ControllerBase
    {
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