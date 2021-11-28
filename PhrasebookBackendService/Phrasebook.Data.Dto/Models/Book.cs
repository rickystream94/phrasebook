using System;
using System.Collections.Generic;

namespace Phrasebook.Data.Dto.Models
{
    public class Book
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public string FirstLanguageName { get; set; }

        public string ForeignLanguageName { get; set; }

        public IReadOnlyCollection<Phrase> Phrases { get; set; }
    }
}
