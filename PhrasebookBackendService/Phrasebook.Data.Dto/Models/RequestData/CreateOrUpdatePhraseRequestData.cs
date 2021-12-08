using Phrasebook.Common;
using System.Linq;

namespace Phrasebook.Data.Dto.Models.RequestData
{
    public class CreateOrUpdatePhraseRequestData
    {
        public string FirstLanguagePhrase { get; set; }

        public string ForeignLanguagePhrase { get; set; }

        public string LexicalItemType { get; set; }

        public string[] ForeignLanguageSynonyms { get; set; }

        public string Description { get; set; }

        public void SanitizeAll()
        {
            this.FirstLanguagePhrase = this.FirstLanguagePhrase?.Sanitize();
            this.ForeignLanguagePhrase = this.ForeignLanguagePhrase?.Sanitize();
            this.LexicalItemType = this.LexicalItemType?.Sanitize();
            this.ForeignLanguageSynonyms = this.ForeignLanguageSynonyms?.Select(s => s.Sanitize()).ToArray();
            this.Description = this.Description?.Sanitize();
        }     
    }
}
