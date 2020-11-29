using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Femicides.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Femicides.API.Controllers
{
    public class CityController : ApiController // todo: const select statement
    {
        public class CityReturnModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string FilterUrl { get; set; }
            public string Url { get; set; }
            public int VictimCount { get; set; }
        }
        private readonly IMemoryCache memoryCache;
        public CityController(IMemoryCache memCache) => memoryCache = memCache;

        List<CityReturnModel> cities = new List<CityReturnModel>();

        [NonAction] // todo repo
        public async Task<List<CityReturnModel>> GetAllCityAsync()
        {
            if(!memoryCache.TryGetValue("cities", out cities))
            {
                cities = await Context.City.Include(x => x.Victim).Select(s => new CityReturnModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    FilterUrl = s.FilterUrl,
                    Url = s.Url,
                    VictimCount = s.Victim.Count
                }).ToListAsync();
                memoryCache.Set("cities", cities, System.DateTime.Now.AddDays(1));
            }
            return cities;
        }
        public async Task<IActionResult> GetAllByFilters([FromQuery] string name, [FromQuery] int most, [FromQuery] int least, [FromQuery] int page)
        {
            if (page == 0) page = 1;
            page = System.Math.Abs(page);
            if(page > 9)// cities's max page count : 9
            {
                return Warn(204);
            }

            bool IsNeedPagination = true;
            cities = await GetAllCityAsync();
            var requestedQueries = Request.Query.ToArray();

            if(requestedQueries.AreThereNecessaryQueries())
            {
                if(most > 0 || least > 0)
                {
                    if(!string.IsNullOrEmpty(name))
                    {
                        return Warn(400, "You can not use name filter with most or least filters, please look at the docs.");
                    }
                    if(most > 0 && least > 0)
                    {
                        return Warn(400, "You can not use most and least filters at the same time, please look at the docs.");
                    }
                    if(most > 0)
                    {
                        cities = cities.OrderByDescending(x => x.VictimCount).Take(most).ToList();
                    }
                    else
                    {
                        cities = cities.OrderBy(x => x.VictimCount).Take(least).ToList();
                    }
                    IsNeedPagination = false;
                }
                else if(!string.IsNullOrEmpty(name))
                {
                    cities = cities.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
                }
            }
            var citiesCountBeforeSkip = cities.Count;
            if(citiesCountBeforeSkip > 0)
            {
                if(citiesCountBeforeSkip > maxDataCountPerPage && IsNeedPagination)
                {
                    cities = cities.Skip(maxDataCountPerPage * (page - 1)).Take(maxDataCountPerPage).ToList();
                }
                return IsNeedPagination ? Succes(null, cities, Information(citiesCountBeforeSkip,page,requestedQueries)) : Succes(null, cities);
            }
            return Warn(204);
        }

        [HttpGet("{Ids}")]
        public async Task<IActionResult> GetMultipleByIds([FromRoute]MultipleModel value)
        {
            if (!ModelState.IsValid)
            {
                return Warn(400, ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault().ErrorMessage);
            }
            cities = await GetAllCityAsync();
            var idsArr = value.Ids.Split(",");
            var requestedCities = new List<CityReturnModel>();

            foreach (var item in idsArr)
            {
                if(requestedCities.Any(x => x.Id == int.Parse(item)))
                {
                    continue;
                }

                var city = cities.Where(x => x.Id == int.Parse(item)).FirstOrDefault();

                if (city != null)
                {
                    requestedCities.Add(city);
                }
            }

            return requestedCities.Count > 0 ? Succes(null, requestedCities) : Warn(204);
        }

        [HttpGet("{value:int}")]
        public async Task<IActionResult> GetSingleById([FromRoute]int value)
        {
            cities = await GetAllCityAsync();
            var city = cities.FirstOrDefault(x=> x.Id == value);

            return city != null ? Succes(null, city) : Warn(204);
        }
    }
}