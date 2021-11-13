namespace Phrasebook.Data.Dto.Models.RequestData
{
    public class CreateOrUpdatePhrasebookRequestData
    {
        public string FirstLanguageCode { get; set; }

        public string ForeignLanguageCode { get; set; }

        public override string ToString()
        {
            return $"First language code: {this.FirstLanguageCode}; Foreign language code: {this.ForeignLanguageCode}";
        }
    }
}
