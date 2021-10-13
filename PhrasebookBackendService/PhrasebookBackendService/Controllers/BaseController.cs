using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Phrasebook.Common;
using Phrasebook.Data;

namespace PhrasebookBackendService.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        public BaseController(ILogger<BaseController> logger, PhrasebookDbContext dbContext, ITimeProvider timeProvider)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.TimeProvider = timeProvider ?? throw new ArgumentNullException(nameof(TimeProvider));
        }

        protected ILogger<BaseController> Logger { get; }

        protected PhrasebookDbContext DbContext { get; }

        protected ITimeProvider TimeProvider { get; }
    }
}
