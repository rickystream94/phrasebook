using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Phrasebook.Common;
using Phrasebook.Data;
using Phrasebook.Data.Dto;

namespace PhrasebookBackendService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhrasebooksController : BaseController
    {
        private readonly Expression<Func<Phrasebook.Data.Models.Book, object>>[] propertiesForListResult = new Expression<Func<Phrasebook.Data.Models.Book, object>>[]
        {
            b => b.FirstLanguage,
            b => b.ForeignLanguage,
            b => b.User
        };

        private readonly Expression<Func<Phrasebook.Data.Models.Book, object>>[] propertiesForSingleResult = new Expression<Func<Phrasebook.Data.Models.Book, object>>[]
        {
            b => b.FirstLanguage,
            b => b.ForeignLanguage,
            b => b.User,
            b => b.Phrases,
        };

        public PhrasebooksController(
            ILogger<PhrasebooksController> logger,
            PhrasebookDbContext dbContext,
            ITimeProvider timeProvider)
            : base(logger, dbContext, timeProvider)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetPhrasebooksAsync()
        {
            IEnumerable<Phrasebook.Data.Models.Book> phrasebooks = await this.DbContext
                .GetEntitiesAsync(navigationPropertiesToInclude: this.propertiesForListResult);
            this.Logger.LogInformation($"Retrieved {phrasebooks.Count()} phrasebooks.");
            return Ok(phrasebooks
                .Select(b => b.ToPhrasebookDto())
                .ToListResult());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPhrasebookAsync([FromRoute] int id)
        {
            Phrasebook.Data.Models.Book phrasebook = await this.DbContext.GetEntityByIdAsync(id, this.propertiesForSingleResult);

            if (phrasebook == null)
            {
                return this.NotFound();
            }

            return Ok(phrasebook.ToPhrasebookDto());
        }
    }
}
