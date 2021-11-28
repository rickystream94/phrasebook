using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Phrasebook.Data.Models
{
    public class Book : EntityBase
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public string FirstLanguageId { get; set; }

        public string ForeignLanguageId { get; set; }

        public DateTime CreatedOn { get; set; }

        public User User { get; set; }

        public Language FirstLanguage { get; set; }

        public Language ForeignLanguage { get; set; }

        public ICollection<Phrase> Phrases { get; set; }
    }
}
