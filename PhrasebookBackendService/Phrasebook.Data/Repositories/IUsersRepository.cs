using Phrasebook.Data.Models;
using System;
using System.Threading.Tasks;

namespace Phrasebook.Data.Repositories
{
    public interface IUsersRepository : IGenericRepository<User>
    {
        Task<User> GetUserByPrincipalIdAsync(Guid principalId);

        Task<User> UpdateUserDisplayNameAsync(Guid principalId, string newDisplayName);

        Task<User> CreateNewUserAsync(string identityProvider, Guid principalId, string email, string displayName, string fullName);

        Task<User> DeleteUserAsync(Guid principalId);
    }
}
