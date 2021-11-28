using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Phrasebook.Data.Models
{
    public class User : EntityBase
    {
        public int Id { get; set; }

        [Required]
        [StringLength(64, MinimumLength = 5)]
        public string Email { get; set; }

        [StringLength(50)]
        public string DisplayName { get; set; }

        [StringLength(100)]
        public string FullName { get; set; }

        [StringLength(10)]
        public string IdentityProvider { get; set; }

        public Guid PrincipalId { get; set; }

        public DateTime SignedUpOn { get; set; }

        public ICollection<Book> Phrasebooks { get; set; }
    }
}
