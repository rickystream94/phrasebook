using System.ComponentModel.DataAnnotations;

namespace Phrasebook.Data.Models
{
    public class Language : EntityBase
    {
        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string DisplayName { get; set; }
    }
}
