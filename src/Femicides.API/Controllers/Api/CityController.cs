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
    public class CityController : ApiController // todo: const select statement
    {
        private readonly IMemoryCache memoryCache;
        public CityController(IMemoryCache memCache) => memoryCache = memCache;

        [NonAction] // todo repo
        public async Task<List<City>> GetAllCityAsync()
        {
            var cities = new List<City>();
            if(!memoryCache.TryGetValue("cities", out cities))
            {
                cities = await Context.City.Include(x => x.Victim).ToListAsync();
                memoryCache.Set("cities", cities, MemoryCacheExpOptions);
            }

            return cities;
        }

        public async Task<IActionResult> GetAllByFilters([FromQuery] string name, [FromQuery] int most, [FromQuery] int least, [FromQuery] int page = 1)
        {
            bool IsNeedPagination = true;
            sbyte maxCityPageCount = 9;
            page = System.Math.Abs(page);

            if(page > maxCityPageCount)
            {
                return Error(404);
            }
            var cities = await GetAllCityAsync();
            var requestedQueries = Request.Query.ToArray();

            if(Request.QueryString.HasValue)
            {
                if(requestedQueries.Count() == 1 && requestedQueries[0].Key.ToLower() == "page") //todo: fix duplicated
                {
                    goto breakfilter;
                }

                if(most > 0 || least > 0)
                {
                    IsNeedPagination = false;
                    if(!string.IsNullOrEmpty(name))
                        return Error(400, "You can not use name filter with most or least filters, please look at the docs.");
                    if(most > 0 && least > 0)
                        return Error(400, "You can not use most and least filters at the same time, please look at the docs.");

                    if(most > 0)
                        cities = cities.OrderByDescending(x => x.Victim.Count).Take(most).ToList();
                    else
                        cities = cities.OrderBy(x => x.Victim.Count).Take(least).ToList();
                }
                else
                {
                    if(!string.IsNullOrEmpty(name))
                    {
                        cities = cities.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
                    }
                }
            }
            breakfilter:
            var citiesCountBeforeSkip = cities.Count;

            if(citiesCountBeforeSkip > 0)
            {
                if(citiesCountBeforeSkip > maxDataCountPerPage && IsNeedPagination)
                {
                    cities = cities.Skip(maxDataCountPerPage * (page - 1)).Take(maxDataCountPerPage).ToList();
                }

                var returnData = cities.Select(s => new //todo const select statement
                {
                    s.Id,
                    s.Name,
                    s.FilterUrl,
                    s.Url,
                    victimCount = s.Victim.Count
                }).ToList();

                if(IsNeedPagination)
                    return Succes(null, returnData, Pagination(citiesCountBeforeSkip,page,requestedQueries.ToStringQueries()));
                else
                    return Succes(null, returnData);
            }

            return Error(404);
        }

        [HttpGet("{Ids}")]
        public async Task<IActionResult> GetMultipleByIds([FromRoute]MultipleModel value)
        {
            if (!ModelState.IsValid)
            {
                return Error(400, ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault().ErrorMessage);
            }

            var cities = await GetAllCityAsync();
            var idsArr = value.Ids.Split(",");
            var requestedCities = new List<City>();

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

            if (requestedCities.Count > 0)
            {
                var returnData = requestedCities.Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.FilterUrl,
                    s.Url,
                    victimCount = s.Victim.Count
                }).ToList();

                return Succes(null, returnData);
            }

            return Error(404);
        }

        [HttpGet("{value:int}")]
        public async Task<IActionResult> GetSingleById([FromRoute]int value)
        {
            var cities = await GetAllCityAsync();
            var city = cities.Where(x=> x.Id == value).Select(s => new
            {
                s.Id,
                s.Name,
                s.FilterUrl,
                s.Url,
                victimCount = s.Victim.Count
            }).FirstOrDefault();
            if(city != null)
            {
                return Succes(null, city);
            }
            return Error(404);
        }
    }
}