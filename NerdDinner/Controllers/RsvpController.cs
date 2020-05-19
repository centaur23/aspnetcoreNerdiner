using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NerdDinner.Data;
using NerdDinner.Models;

namespace NerdDinner.Controllers
{
    public class RsvpController : Controller
    {
        private readonly NerdDinnerContext _context;
        private readonly INerdDinnerRepository _repository;
        private readonly UserManager<IdentityUser> _userManager;

        public RsvpController(NerdDinnerContext context, INerdDinnerRepository repository, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _repository = repository;
            _userManager = userManager;

        }

        [HttpGet("isUserHost")]
        [AllowAnonymous]
        public IActionResult IsUserHost(int id)
        {
            if (_userManager.GetUserId(User) == null)
            {
                return new ObjectResult(false);
            }

            var dinner = _repository.GetDinner(id);
            var user = _userManager.GetUserId(User);
            return new ObjectResult(dinner.IsUserHost(user));
        }


        // GET: Rsvp
        public async Task<IActionResult> Index()
        {
            var nerdDinnerContext = _context.Rsvps.Include(r => r.Dinner);
            return View(await nerdDinnerContext.ToListAsync());
        }

        // GET: Rsvp/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rsvp = await _context.Rsvps
                .Include(r => r.Dinner)
                .FirstOrDefaultAsync(m => m.RsvpId == id);
            if (rsvp == null)
            {
                return NotFound();
            }

            return View(rsvp);
        }

        // GET: Rsvp/Create
        public IActionResult Create(long id)
        {
            Rsvp rsvp = new Rsvp { DinnerId = id, UserName = _userManager.GetUserName(User) };
            _context.Add(rsvp);
            _context.SaveChanges();
            return View(rsvp);
        }

        
        // GET: Rsvp/Edit/5
        [Authorize]
        public IActionResult Edit(long? id)
        {
            var rsvp = _context.Rsvps.Find(id);
            var user = _userManager.GetUserName(User);
            if (rsvp.UserName != user)
            {
                return View("~/Views/Dinner/permission.cshtml");
            }

            if (id == null)
            {
                return NotFound();
            }

            //var rsvp = await _context.Rsvps.FindAsync(id);
            if (rsvp == null)
            {
                return NotFound();
            }
            ViewData["DinnerId"] = new SelectList(_context.Dinners, "DinnerId", "Address", rsvp.DinnerId);
            return View(rsvp);
        }

        // POST: Rsvp/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(long id, [Bind("RsvpId,DinnerId,UserName")] Rsvp rsvp)
        {
            if (id != rsvp.RsvpId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rsvp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RsvpExists(rsvp.RsvpId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DinnerId"] = new SelectList(_context.Dinners, "DinnerId", "Address", rsvp.DinnerId);
            return View(rsvp);
        }

        // GET: Rsvp/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var rsv = _context.Rsvps.Find(id);
            var user = _userManager.GetUserName(User);
            if (rsv.UserName != user)
            {
                return View("~/Views/Dinner/permission.cshtml");
            }

            var rsvp = await _context.Rsvps
                .Include(r => r.Dinner)
                .FirstOrDefaultAsync(m => m.RsvpId == id);
            if (rsvp == null)
            {
                return NotFound();
            }

            return View(rsvp);
        }

        // POST: Rsvp/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var rsvp = await _context.Rsvps.FindAsync(id);
            _context.Rsvps.Remove(rsvp);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RsvpExists(long id)
        {
            return _context.Rsvps.Any(e => e.RsvpId == id);
        }
    }
}
