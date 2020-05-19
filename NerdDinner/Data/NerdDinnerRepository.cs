using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NerdDinner.Models;

namespace NerdDinner.Data
{
    public class NerdDinnerRepository : INerdDinnerRepository
    {
        private readonly NerdDinnerContext _context;

        public NerdDinnerRepository(NerdDinnerContext context)
        {
            _context = context;
        }

        public Dinner GetDinner(long dinnerId)
        {
            return _context.Dinners
                .Include(d => d.Rsvps)
                .SingleOrDefault(d => d.DinnerId == dinnerId);

        }

        public List<Dinner> GetDinnersList()
        {
            return _context.Dinners
                .Include(d => d.Rsvps)
                .OrderByDescending(d => d.Rsvps.Count)
                .ToList();
        }

        public Dinner CreateDinner(Dinner dinner)
        {
            var rsvp = new Rsvp
            {
                UserName = dinner.UserName
            };

            dinner.Rsvps = new List<Rsvp> { rsvp };

            _context.Add(dinner);
            _context.Add(rsvp);
            _context.SaveChanges();

            return dinner;
        }

        public Dinner UpdateDinner(Dinner dinner)
        {
            
            _context.Update(dinner);
            _context.SaveChanges();
            return dinner;
        }

        public void DeleteDinner(long id)
        {
            var dinner = _context.Dinners.Find(id);
            _context.Dinners.Remove(dinner);
            _context.SaveChanges();
        }


        public Rsvp CreateRsvp(long dinnerId, string userName)
        {
            Rsvp rsvp = null;

            var dinner = GetDinner(dinnerId);
            if (dinner != null)
            {
                if (dinner.IsUserRegistered(userName))
                {
                    rsvp = dinner.Rsvps.SingleOrDefault(r => string.Equals(r.UserName, userName, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    rsvp = new Rsvp
                    {
                        UserName = userName
                    };

                    dinner.Rsvps.Add(rsvp);
                    _context.Add(rsvp);
                    _context.SaveChanges();
                }
            }

            return rsvp;
        }

        public void DeleteRsvp(Dinner dinner, string userName)
        {
            var rsvp = dinner?.Rsvps.SingleOrDefault(r => string.Equals(r.UserName, userName, StringComparison.OrdinalIgnoreCase));
            if (rsvp != null)
            {
                _context.Rsvps.Remove(rsvp);
                _context.SaveChanges();
            };

        }

        public List<Rsvp> GetRsvpList()
        {
            return _context.Rsvps.ToList();
        }

        
    }       
}
