using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Phrasebook.Data.Models
{
    public class Book : EntityBase
    {
        [Required]
        public int UserId { get; set; }

        public int? FirstLanguageId { get; set; }

        public int? ForeignLanguageId { get; set; }

        public DateTime CreatedOn { get; set; }

        public User User { get; set; }

        public Language FirstLanguage { get; set; }

        public Language ForeignLanguage { get; set; }

        public ICollection<Phrase> Phrases { get; set; }
    }
}
