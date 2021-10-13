using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Phrasebook.Data.Models
{
    public class Language : EntityBase
    {
        [Required]
        [StringLength(50)]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string Code { get; set; }
    }
}
