using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Phrasebook.Common;
using Phrasebook.Common.Constants;
using Phrasebook.Data.Dto;
using Phrasebook.Data.Models;
using Phrasebook.Data.Sql;
using PhrasebookBackendService.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhrasebookBackendService.Controllers
{
    [ApiController]
    [Authorize(Policy = Constants.UserIsSignedUpPolicy)]
    [Route("api/[controller]")]
    public class LanguageController : BaseController
    {
        public LanguageController(
            ILogger<LanguageController> logger,
            IUnitOfWork unitOfWork,
            ITimeProvider timeProvider,
            IValidatorFactory validatorFactory)
            : base(logger, unitOfWork, timeProvider, validatorFactory)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetSupportedLanguagesAsync()
        {
            IEnumerable<Language> languages = await this.UnitOfWork.LanguageRepository.GetEntitiesAsync();
            return this.Ok(languages.ToListResult());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupportedLanguageByIdAsync([FromRoute] string id)
        {
            Language language = await this.UnitOfWork.LanguageRepository.GetEntityAsync(l => l.Id == id.ToLower());
            if (language == null)
            {
                return this.NotFound();
            }

            return this.Ok(language);
        }
    }
}
