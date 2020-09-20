using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Femicides.API.Extensions;
using Femicides.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Femicides.API.Controllers
{
    public class CityController : ApiController
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

        public async Task<IActionResult> GetAllByFilters([FromQuery] string name,[FromQuery] int page = 1)
        {
            sbyte maxCityPageCount = 9;
            page = System.Math.Abs(page);
            if(page > maxCityPageCount)
            {
                return Error(404);
            }
            var cities = await GetAllCityAsync();
            KeyValuePair<string, StringValues>[] requestedQueries = null;
            if(Request.QueryString.HasValue)
            {
                requestedQueries = Request.Query.ToArray();
                if(requestedQueries.Count() == 1 && requestedQueries[0].Key.ToLower() == "page")
                {
                    goto breakfilter;
                }

                if(!string.IsNullOrEmpty(name))
                {
                    cities = cities.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
                }
            }
            breakfilter:
            var citiesCountBeforeSkip = cities.Count;

            if(citiesCountBeforeSkip > 0)
            {
                if(citiesCountBeforeSkip > maxDataCountPerPage)
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

                return Succes(null, returnData, Pagination(citiesCountBeforeSkip,page,requestedQueries.ToStringQueries()));
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