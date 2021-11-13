namespace Phrasebook.Data.Dto.Models.RequestData
{
    public class CreateOrUpdatePhraseRequestData
    {
        public string FirstLanguagePhrase { get; set; }

        public string ForeignLanguagePhrase { get; set; }

        public LexicalItemType? LexicalItemType { get; set; }

        public string[] ForeignLanguageSynonyms { get; set; }

        public string Description { get; set; }
    }
}
