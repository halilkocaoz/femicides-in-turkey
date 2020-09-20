using System.ComponentModel.DataAnnotations;

namespace Femicides.API
{
    public class MultipleModel
    {
        [Required]
        [RegularExpression(@"[0-9,]+", ErrorMessage = "Multiple request REGEX is [0-9,]+")]
        public string Ids { get; set; }
    }
}