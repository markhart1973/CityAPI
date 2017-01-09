using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models
{
    // TODO Convert validation to FluentValidation.
    public class PointOfInterestForUpdateDto
    {
        [MaxLength(50, ErrorMessage = "The field Name must be a string or array type with a maximum length of '200'.")]
        public string Name { get; set; }

        [MaxLength(200, ErrorMessage = "The field Description must be a string or array type with a maximum length of '200'.")]
        public string Description { get; set; }
    }
}
