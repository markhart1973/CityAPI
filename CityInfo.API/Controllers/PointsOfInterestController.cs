using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController :
        Controller
    {
        private const string GetActionAllPoi = "GetPointsOfInterest";
        private const string GetActionPoi = "GetPointOfInterest";

        [HttpGet("{cityId}/pointsofinterest", Name = GetActionAllPoi)]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            var city = CitiesDataStore.Current.Cities
                .FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{cityId}/pointsofinterest/{id}", Name = GetActionPoi)]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities
                .FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            var poi = city.PointsOfInterest
                .FirstOrDefault(p => p.Id == id);
            if (poi == null)
            {
                return NotFound();
            }
            return Ok(poi);
        }

        [HttpPost("{cityId}/pointsofinterest")]
        public IActionResult CreatePointOfInterest(int cityId,
            [FromBody] PointOfInterestForCreationDto poi)
        {
            if (poi == null)
            {
                return BadRequest();
            }

            if (poi.Description == poi.Name)
            {
                this.ModelState.AddModelError("Description", "The provided description should be different from the name.");
            }

            if (!this.ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = CitiesDataStore.Current.Cities
                .FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var maxPoiId = CitiesDataStore.Current.Cities
                .SelectMany(c => c.PointsOfInterest)
                .Max(p => p.Id);
            var finalPoi = new PointOfInterestDto
            {
                Id = ++maxPoiId,
                Name = poi.Name,
                Description = poi.Description
            };
            city.PointsOfInterest.Add(finalPoi);

            return CreatedAtRoute(GetActionPoi,
                new { cityId = cityId, id = finalPoi.Id },
                finalPoi);
        }

        [HttpPost("{cityId}/pointsofinterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId,
            int id,
            [FromBody] PointOfInterestForUpdateDto poi)
        {
            if (poi == null)
            {
                return BadRequest();
            }

            if (poi.Description == poi.Name)
            {
                this.ModelState.AddModelError("Description", "The provided description should be different from the name.");
            }

            if (!this.ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentCity = CitiesDataStore.Current.Cities
                .FirstOrDefault(c => c.Id == cityId);
            if (currentCity == null)
            {
                return NotFound();
            }

            var currentPoi = currentCity.PointsOfInterest
                .FirstOrDefault(c => c.Id == id);
            if (currentPoi == null)
            {
                return NotFound();
            }

            currentPoi.Name = poi.Name;
            currentPoi.Description = poi.Description;

            return NoContent();
        }

        [HttpPatch("{cityId}/pointsofinterest/{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId,
            int id,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var currentCity = CitiesDataStore.Current.Cities
                .FirstOrDefault(c => c.Id == cityId);
            if (currentCity == null)
            {
                return NotFound();
            }

            var currentPoi = currentCity.PointsOfInterest
                .FirstOrDefault(c => c.Id == id);
            if (currentPoi == null)
            {
                return NotFound();
            }

            var patchPoi = new PointOfInterestForUpdateDto
            {
                Name = currentPoi.Name,
                Description = currentPoi.Description
            };

            patchDoc.ApplyTo(patchPoi, this.ModelState);

            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            if (patchPoi.Description == patchPoi.Name)
            {
                this.ModelState.AddModelError("Description", "The provided description should be different from the name.");
            }

            this.TryValidateModel(patchPoi);

            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            currentPoi.Name = patchPoi.Name;
            currentPoi.Description = patchPoi.Description;

            return NoContent();
        }
    }
}
