using Phrasebook.Data.Models;
using Phrasebook.Data.Sql;

namespace Phrasebook.Data.Repositories
{
    public class UsersRepository : GenericRepository<User>, IUsersRepository
    {
        public UsersRepository(PhrasebookDbContext context)
            : base(context)
        {
        }
    }
}
