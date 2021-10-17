using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Phrasebook.Common;
using Phrasebook.Data;
using Phrasebook.Data.Validation;
using PhrasebookBackendService.EasyAuth;

namespace PhrasebookBackendService.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        public BaseController(ILogger<BaseController> logger, PhrasebookDbContext dbContext, ITimeProvider timeProvider, IValidatorFactory validatorFactory)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.TimeProvider = timeProvider ?? throw new ArgumentNullException(nameof(TimeProvider));
            this.ValidatorFactory = validatorFactory ?? throw new ArgumentNullException(nameof(validatorFactory));
            this.AuthenticatedUserLazy = new Lazy<AuthenticatedUser>(this.GetAuthenticatedUser);
        }

        protected ILogger<BaseController> Logger { get; }

        protected IValidatorFactory ValidatorFactory { get; }

        protected PhrasebookDbContext DbContext { get; }

        protected ITimeProvider TimeProvider { get; }

        protected AuthenticatedUser AuthenticatedUser => this.AuthenticatedUserLazy.Value;

        private Lazy<AuthenticatedUser> AuthenticatedUserLazy { get; }

        private AuthenticatedUser GetAuthenticatedUser()
        {
            return AuthenticatedUser.FromClaimsPrincipal(this.Request.HttpContext.User);
        }
    }
}
