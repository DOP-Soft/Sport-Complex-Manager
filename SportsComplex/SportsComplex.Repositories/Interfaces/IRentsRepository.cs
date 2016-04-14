using SportsComplex.Entities;
using System;
using System.Collections.Generic;

namespace SportsComplex.Repositories
{
    interface IRentsRepository
    {
        IEnumerable<RentItem> SelectAll();

        IEnumerable<RentItem> GetRentsOnDate(DateTime inputDate);

        IEnumerable<RentItem> GetRentsByCustomerId(int renterId);

        RentItem GetRentById(int rentId);

        int MakeRent(int renterId, int sportsHallId, DateTime dateStart, DateTime dateEnd, decimal Cost);

        void ExtendRent(int rentId, DateTime newEndDate, decimal overpay);

        // Returns money change
        decimal Remove(int rentId);
    }
}
