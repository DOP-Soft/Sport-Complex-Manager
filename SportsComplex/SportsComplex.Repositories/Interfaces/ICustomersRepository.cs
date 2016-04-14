using SportsComplex.Entities;
using System.Collections.Generic;

namespace SportsComplex.Repositories
{
    interface ICustomersRepository
    {
        IEnumerable<Customer> SelectAll();

        Customer GetById(int renterId);

        int Add(string lastName, string firstName, string phone);

        void Edit(int renterId, string lastName, string firstName, string phone);

        void Remove(int renterId); 
    }
}
