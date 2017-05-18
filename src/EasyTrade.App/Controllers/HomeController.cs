using EasyTrade.App.Models;
using EasyTrade.App.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Spatial;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EasyTrade.App.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// The location of New York (used for demo purposes)
        /// </summary>
        private static readonly Point NewYorkPoint = new Point(-74.0059700, 40.7142700);

        public IActionResult Index()
        {
            var claims = ((ClaimsIdentity) User.Identity).Claims;

            return View(claims);
        }
        
        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Latency()
        {
            return View();
        }


        /// <summary>
        /// Spatial Linq query.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The ZipCodeEntry instances that are within 2km from the point of reference</returns>
        public async Task<IList<ZipCodeEntry>> ZipSearch(ZipEntrySearchInput input)
        {
            // Use New York as point of reference if not provided.
            var samplePoint = NewYorkPoint;

            if (input.Latitude.HasValue && input.Longitude.HasValue)
            {
                samplePoint = new Point(input.Longitude.Value, input.Latitude.Value);
            }

            var zipRepo = new ZipCodeRepository();
            var zipsWithin = await zipRepo.GetItemsAsync(ze => ze.Location.Distance(samplePoint) < 2000, enableScanInQuery: true);
            return zipsWithin.ToList();
        }

        /// <summary>
        /// Spatial Linq query with projection to anonymous object.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The ZipCodeEntry instances that are within 2km from the point of reference</returns>
        public async Task<IEnumerable<object>> ZipSearchCity(ZipEntrySearchInput input)
        {
            // Use New York as point of reference if not provided.
            var samplePoint = NewYorkPoint;

            if (input.Latitude.HasValue && input.Longitude.HasValue)
            {
                samplePoint = new Point(input.Longitude.Value, input.Latitude.Value);
            }

            var zipRepo = new ZipCodeRepository();
            var zipsWithin = await zipRepo
                .GetQuery(query =>
                    query.Where(ze => ze.Location.Distance(samplePoint) < 2000)
                         .Select(ze => new { ze.Id, ze.City }), enableScanInQuery: true);
            return zipsWithin.ToList();
        }

        [HttpPost]
        public async Task<long> CheckLatency()
        {
            var zipRepo = new ZipCodeRepository();
            var sw = Stopwatch.StartNew();
            var count = 50;
            for (int i = 0; i < count; i++)
            {
                // Fetch a guaranteed to be random document from the collection
                var randomId = "98625";
                var zipDocument = await zipRepo.GetItemAsync(randomId);
            }

            return sw.ElapsedMilliseconds / count;
        }
    }
}