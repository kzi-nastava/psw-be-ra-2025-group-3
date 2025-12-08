using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Stakeholders.Core.UseCases; 
using Explorer.Stakeholders.API.Dtos;      
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/tour-previews")]
    [ApiController]
    public class TourPreviewController : ControllerBase
    {
        private readonly ITouristTourService _touristTourService;
        private readonly IPersonService _personService;

        public TourPreviewController(ITouristTourService touristTourService, IPersonService personService)
        {
            _touristTourService = touristTourService;
            _personService = personService;
        }

        [HttpGet]
        public ActionResult<List<TourPreviewDto>> GetPublishedTours()
        {
            var result = _touristTourService.GetPublishedTours();

            foreach (var tour in result)
            {
                foreach (var review in tour.Reviews)
                {
                    try
                    {
                        var person = _personService.Get(review.TouristId);
                        review.TouristName = person.Name + " " + person.Surname;
                    }
                    catch (Exception)
                    {
                        review.TouristName = "Anonymous";
                    }
                }
            }

            return Ok(result);
        }
    }
}