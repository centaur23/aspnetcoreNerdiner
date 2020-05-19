using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NerdDinner.Models;

namespace NerdDinner.Data
{
    public interface INerdDinnerRepository
    {
        Dinner CreateDinner(Dinner dinner);
        List<Dinner> GetDinnersList();
        Dinner GetDinner(long dinnerId);
        Dinner UpdateDinner(Dinner dinner);
        void DeleteDinner(long id);

        Rsvp CreateRsvp(long dinnerId, string userName);

        void DeleteRsvp(Dinner dinner, string userName);

        List<Rsvp> GetRsvpList();

    }
}
