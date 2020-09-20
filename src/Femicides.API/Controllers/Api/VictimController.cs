using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Femicides.API.Extensions;
using Femicides.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Femicides.API.Controllers
{
    public class VictimController : ApiController
    {
        private readonly IMemoryCache memoryCache;
        public VictimController(IMemoryCache memCache) => memoryCache = memCache;

        [NonAction]
        public async Task<List<Victim>> GetVictims()
        {
            var victims = new List<Victim>();
            if(!memoryCache.TryGetValue("victims", out victims))
            {
                victims = await Context.Victim.
                Include(victim => victim.VictimCausesOfKilled).
                Include(victim => victim.VictimMethodsOfKilled).
                Include(victim => victim.Perpetrator).
                Include(victim => victim.City).
                OrderByDescending(victim => victim.Id).ToListAsync();

                memoryCache.Set("victims", victims, MemoryCacheExpOptions);
            }

            return victims;
        }
        public async Task<IActionResult> GetAllByFilters(
            [FromQuery] string name, [FromQuery] string surname, [FromQuery] string city,
            [FromQuery] bool? adult, [FromQuery] bool? protectionRequest, [FromQuery] string killer,
            [FromQuery] string method, [FromQuery] string cause, [FromQuery] string year, [FromQuery] int page = 1)
        {
            const int maxVictimPageCount = 400;
            page = System.Math.Abs(page);
            if (page > maxVictimPageCount)
            {
                return Error(404);
            }
            var victims = await GetVictims();

            var requestedQueries = Request.Query.ToArray();;
            if(Request.QueryString.HasValue)
            {
                if(requestedQueries.Count() == 1 && requestedQueries[0].Key.ToLower() == "page") //todo: fix duplicated
                {
                    goto breakfilter;
                }
                #region Filter
                if(!string.IsNullOrEmpty(name))
                {
                    victims = victims.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
                }
                if(!string.IsNullOrEmpty(surname) && victims.Count > 0)
                {
                    victims = victims.Where(x => x.Surname.ToLower().Contains(surname.ToLower())).ToList();
                }
                if(!string.IsNullOrEmpty(city) && victims.Count > 0)
                {
                    victims = victims.Where(x => x.City.Name.ToLower().Contains(city.ToLower())).ToList();
                }
                if(adult.HasValue && victims.Count > 0)
                {
                    victims = victims.Where(x => x.Adult == adult.Value).ToList();
                }
                if(protectionRequest.HasValue && victims.Count > 0)
                {
                    victims = victims.Where(x => x.ProtectionRequest == protectionRequest.Value).ToList();
                }
                if(!string.IsNullOrEmpty(method) && victims.Count > 0)
                {
                    victims = victims.Where(x => x.VictimMethodsOfKilled.Any(x => x.Method.ToLower() == method.ToLower())).ToList();
                }
                if(!string.IsNullOrEmpty(year) && victims.Count > 0)
                {
                    victims = victims.Where(x => x.Date.Year.ToString() == year).ToList();
                }
                if(!string.IsNullOrEmpty(cause) && victims.Count > 0)
                {
                    victims = victims.Where(x => x.VictimCausesOfKilled.Any(x => x.Cause.ToLower() == cause.ToLower())).ToList();
                }
                if(!string.IsNullOrEmpty(killer) && victims.Count > 0)
                {
                    victims = victims.Where(x => x.Perpetrator.Definition.ToLower().Contains(killer.ToLower())).ToList();
                }
                #endregion
            }
            breakfilter:
            var victimsCountBeforeSkip = victims.Count;
            if (victimsCountBeforeSkip > 0)
            {
                if(victimsCountBeforeSkip > maxDataCountPerPage)
                {
                    victims = victims.Skip(maxDataCountPerPage * (page - 1)).Take(maxDataCountPerPage).ToList();
                }

                var returnData = victims.Select(s => new
                {
                    FullName = s.Name + " " + s.Surname,
                    City = s.City.Name,
                    Killer = new
                    {
                        s.Perpetrator.Definition,
                        s.Perpetrator.Status
                    },
                    Methods = s.VictimMethodsOfKilled.Select(ms => new
                    {
                        ms.Method
                    }).ToArray(),
                    Causes = s.VictimCausesOfKilled.Select(ks => new
                    {
                        ks.Cause
                    }).ToArray(),
                    s.Adult,
                    s.ProtectionRequest,
                    year = s.Date.Year.ToString(),
                    s.Url
                }).ToList();

                return Succes(null, returnData, Pagination(victimsCountBeforeSkip,page,requestedQueries.ToStringQueries()));
            }

            return Error(404);
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