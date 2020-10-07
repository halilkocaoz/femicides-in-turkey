using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Femicides.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Femicides.API.Controllers
{
    public class VictimController : ApiController
    {
        private readonly IMemoryCache memoryCache;
        public VictimController(IMemoryCache memCache) => memoryCache = memCache;
        #region models
        public class PerpetratorReturnModel
        {
            public string Definition { get; set; }
            public string Status { get; set; }
        }
        public class MethodsReturnModel
        {
            public string Method { get; set; }
        }
        public class CausesReturnModel
        {
            public string Cause { get; set; }
        }
        public class VictimReturnModel
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string City { get; set; }
            public PerpetratorReturnModel Killer { get; set; }
            public List<MethodsReturnModel> Methods { get; set; }
            public List<CausesReturnModel> Causes { get; set; }
            public bool? Adult { get; set; }
            public bool? ProtectionRequest { get; set; }
            public int Year { get; set;}
            public string Url { get; set;}
        }
        #endregion

        List<VictimReturnModel> victims = new List<VictimReturnModel>();

        [NonAction]
        public async Task<List<VictimReturnModel>> GetAllVictimAsync()
        {
            if(!memoryCache.TryGetValue("victims", out victims))
            {
                victims = await Context.Victim.
                Include(victim => victim.VictimCausesOfKilled).Include(victim => victim.VictimMethodsOfKilled).
                Include(victim => victim.Perpetrator).
                Include(victim => victim.City).
                OrderByDescending(victim => victim.Id).
                Select(s => new VictimReturnModel
                {
                    Id = s.Id,
                    FullName = s.Name + " " + s.Surname,
                    City = s.City.Name,
                    Killer = new PerpetratorReturnModel { Definition = s.Perpetrator.Definition, Status = s.Perpetrator.Status },
                    Methods = s.VictimMethodsOfKilled.Select(ms => new MethodsReturnModel { Method = ms.Method }).ToList(),
                    Causes = s.VictimCausesOfKilled.Select(ks => new CausesReturnModel { Cause = ks.Cause }).ToList(),
                    Adult = s.Adult,
                    ProtectionRequest = s.ProtectionRequest,
                    Year = s.Date.Year,
                    Url = s.Url
                }).ToListAsync();
                memoryCache.Set("victims", victims, System.DateTime.Now.AddDays(1));
            }
            return victims;
        }
        public async Task<IActionResult> GetAllByFilters( //todo: name, surname => fullname
            [FromQuery] string name, [FromQuery] string surname, [FromQuery] string city,
            [FromQuery] bool? adult, [FromQuery] bool? protectionRequest, [FromQuery] string killer,
            [FromQuery] string method, [FromQuery] string cause, [FromQuery] int year, [FromQuery] int page)
        {
            if (page == 0) page = 1;
            page = System.Math.Abs(page);
            victims = await GetAllVictimAsync();
            var requestedQueries = Request.Query.ToArray();

            if(requestedQueries.AreThereNecessaryQueries())
            {
                if(!string.IsNullOrEmpty(name))
                {
                    victims = victims.Where(x => x.FullName.ToLower().Contains(name.ToLower())).ToList();
                }
                if(!string.IsNullOrEmpty(surname) && victims.Count > 0)
                {
                    victims = victims.Where(x => x.FullName.ToLower().Contains(surname.ToLower())).ToList();
                }
                if(!string.IsNullOrEmpty(city) && victims.Count > 0)
                {
                    victims = victims.Where(x => x.City.ToLower().Contains(city.ToLower())).ToList();
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
                    victims = victims.Where(x => x.Methods.Any(x => x.Method.ToLower() == method.ToLower())).ToList();
                }
                if(year > 0 && victims.Count > 0)
                {
                    victims = victims.Where(x => x.Year == year).ToList();
                }
                if(!string.IsNullOrEmpty(cause) && victims.Count > 0)
                {
                    victims = victims.Where(x => x.Causes.Any(x => x.Cause.ToLower() == cause.ToLower())).ToList();
                }
                if(!string.IsNullOrEmpty(killer) && victims.Count > 0)
                {
                    victims = victims.Where(x => x.Killer.Definition.ToLower().Contains(killer.ToLower())).ToList();
                }
            }
            var victimsCountBeforeSkip = victims.Count;
            if (victimsCountBeforeSkip > 0)
            {
                if(victimsCountBeforeSkip > maxDataCountPerPage)
                {
                    victims = victims.Skip(maxDataCountPerPage * (page - 1)).Take(maxDataCountPerPage).ToList();
                }

                return Succes(null, victims, Information(victimsCountBeforeSkip,page,requestedQueries));
            }

            return Error(404);
        }

        [HttpGet("{Ids}")]
        public async Task<IActionResult> GetMultipleByIds([FromRoute] MultipleModel value)
        {
            if (!ModelState.IsValid)
            {
                return Error(400, ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault().ErrorMessage);
            }

            victims = await GetAllVictimAsync();
            var idsArr = value.Ids.Split(",");
            var requestedVictims = new List<VictimReturnModel>();

            foreach (var item in idsArr)
            {
                if(requestedVictims.Any(x => x.Id == int.Parse(item)))
                {
                    continue;
                }
                var victim = victims.Where(x => x.Id == int.Parse(item)).FirstOrDefault();
                if (victim != null)
                {
                    requestedVictims.Add(victim);
                }
            }

            return requestedVictims.Count > 0 ? Succes(null, requestedVictims) : Error(404);
        }

        [HttpGet("{value:int}")]
        public async Task<IActionResult> GetSingleById([FromRoute]int value)
        {
            victims = await GetAllVictimAsync();
            var victim = victims.FirstOrDefault(x=> x.Id == value);
            return victim != null ? Succes(null, victim) : Error(404);
        }
    }
}