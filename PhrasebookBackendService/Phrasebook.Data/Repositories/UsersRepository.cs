using Phrasebook.Common;
using Phrasebook.Data.Models;
using Phrasebook.Data.Sql;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Phrasebook.Data.Repositories
{
    public class UsersRepository : GenericRepository<User>, IUsersRepository
    {
        private readonly ITimeProvider timeProvider;

        public UsersRepository(PhrasebookDbContext context, ITimeProvider timeProvider)
            : base(context)
        {
            this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        public async Task<User> GetUserByPrincipalIdAsync(Guid principalId)
        {
            return await this.GetEntityAsync(u => u.PrincipalId == principalId);
        }

        public async Task<User> CreateNewUserAsync(string identityProvider, Guid principalId, string email, string displayName, string fullName)
        {
            User userToCreate = new User
            {
                IdentityProvider = identityProvider,
                PrincipalId = principalId,
                Email = email,
                DisplayName = displayName,
                FullName = fullName,
                SignedUpOn = this.timeProvider.Now,
            };
            await this.Context.ApplyAndSaveChangesAsync(() => this.Add(userToCreate));

            User createdUser = await this.GetUserByPrincipalIdAsync(principalId);
            return createdUser;
        }

        public async Task<User> UpdateUserDisplayNameAsync(Guid principalId, string newDisplayName)
        {
            User userToUpdate = await this.GetUserByPrincipalIdAsync(principalId);
            await this.Context.ApplyAndSaveChangesAsync(() =>
            {
                userToUpdate.DisplayName = newDisplayName;
            });

            return userToUpdate;
        }

        public async Task<User> DeleteUserAsync(Guid principalId)
        {
            User user = await this.GetUserByPrincipalIdAsync(principalId);
            await this.Context.ApplyAndSaveChangesAsync(() => this.Delete(user));
            return user;
        }

        protected override Expression<Func<User, object>>[] GetCommonNavigationProperties()
        {
            return Array.Empty<Expression<Func<User, object>>>();
        }
    }
}
