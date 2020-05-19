using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NerdDinner.Models;

namespace NerdDinner.Data
{
    public class NerdDinnerContext : IdentityDbContext
    {
        public NerdDinnerContext(DbContextOptions<NerdDinnerContext> options)
            : base(options)
        {
        }

        public DbSet<Dinner> Dinners { get; set; }
        public DbSet<Rsvp> Rsvps { get; set; }
    }
}
