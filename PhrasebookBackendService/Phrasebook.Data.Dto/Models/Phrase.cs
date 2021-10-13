using System;
using System.Collections.Generic;

namespace Phrasebook.Data.Dto.Models
{
    public class Phrase
    {
        public string FirstLanguagePhrase { get; set; }

        public string ForeignLanguagePhrase { get; set; }

        public LexicalItemType LexicalItemType { get; set; }

        public IReadOnlyCollection<string> ForeignLanguageSynonyms { get; set; }

        public string Description { get; set; }

        public int CorrectnessCount { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
