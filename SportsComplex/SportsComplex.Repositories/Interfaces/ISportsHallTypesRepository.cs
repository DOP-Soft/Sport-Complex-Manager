using SportsComplex.Entities;
using System.Collections.Generic;

namespace SportsComplex.Repositories
{
    interface ISportsHallTypesRepository
    {
        IEnumerable<SportsHallType> SelectAll();

        int Add(string name);

        void Remove(int id); 
    }
}
