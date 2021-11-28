using Phrasebook.Common.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Phrasebook.Data.Models
{
    public class Phrase : EntityBase
    {
        public int Id { get; set; }

        [Required]
        public int PhrasebookId { get; set; }

        [StringLength(Constants.MaxPhraseLength, MinimumLength = 1)]
        public string FirstLanguagePhrase { get; set; }

        [StringLength(Constants.MaxPhraseLength, MinimumLength = 1)]
        public string ForeignLanguagePhrase { get; set; }

        public LexicalItemType LexicalItemType { get; set; }

        [StringLength(Constants.MaxSynonymsLength)]
        public IEnumerable<string> ForeignLanguageSynonyms { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Range(0, 10)]
        public int CorrectnessCount { get; set; }

        public DateTime CreatedOn { get; set; }

        public Book Phrasebook { get; set; }
    }
}
