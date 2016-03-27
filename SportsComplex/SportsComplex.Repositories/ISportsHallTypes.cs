﻿using SportsComplex.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsComplex.Repositories
{
    interface ISportsHallTypes
    {
        IEnumerable<SportsHallType> SelectAll();

        int Add(string name);

        void Remove(int id); 
    }
}
