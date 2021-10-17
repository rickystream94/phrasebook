using Phrasebook.Data.Models;
using System;
using System.Collections.Generic;

namespace Phrasebook.Data.Dto.Models
{
    public class Book
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public string FirstLanguageDisplayName { get; set; }

        public string ForeignLanguageDisplayName { get; set; }

        public IReadOnlyCollection<Phrase> Phrases { get; set; }
    }
}
