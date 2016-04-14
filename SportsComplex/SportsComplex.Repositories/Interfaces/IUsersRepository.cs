using SportsComplex.Entities;
using System.Collections.Generic;

namespace SportsComplex.Repositories
{
    interface IUsersRepository
    {
        IEnumerable<User> SelectAll();
        
        // Returns user's ID if success.
        int Auth(string Login, string Password);
    }
}
