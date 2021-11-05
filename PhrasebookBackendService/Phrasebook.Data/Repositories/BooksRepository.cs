using Phrasebook.Data.Models;
using Phrasebook.Data.Sql;

namespace Phrasebook.Data.Repositories
{
    public class BooksRepository : GenericRepository<Book>, IBooksRepository
    {
        public BooksRepository(PhrasebookDbContext context)
            : base(context)
        {
        }
    }
}
