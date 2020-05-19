using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NerdDinner.Data;
using NerdDinner.Models;


namespace NerdDinner.Controllers
{
    [Authorize]
    public class DinnerController : Controller
    {
        private readonly NerdDinnerContext _context;
        private readonly INerdDinnerRepository _repository;

        private readonly UserManager<IdentityUser> _userManager;

        public DinnerController(NerdDinnerContext context, INerdDinnerRepository repository, UserManager<IdentityUser> userManager)
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

        [HttpGet("isUserRegistered")]
        [AllowAnonymous]
        public IActionResult IsUserRegistered(int id)
        {
          
            if (_userManager.GetUserId(User) == null)
            {
                return new ObjectResult(false);
            }

            var dinner = _repository.GetDinner(id);
            var UserName = _userManager.GetUserName(User);
            return new ObjectResult(dinner.IsUserRegistered(UserName));
        }



        // GET: Dinner
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(_repository.GetDinnersList());
        }

        // GET: Dinner/Details/5
        [AllowAnonymous]
        public IActionResult Details(long id)
        {
            var dinner = _repository.GetDinner(id);
            if (dinner == null)
            {
                return NotFound();
            }

            return View(dinner);
        }

        // GET: Dinner/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dinner/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Dinner dinner)
        {
            var UserName = _userManager.GetUserName(User);
            dinner.UserName = UserName;
            if (ModelState.IsValid)
            {
                _repository.CreateDinner(dinner);
                return RedirectToAction(nameof(Index));
            }
            return View(dinner);
        }

        // GET: Dinner/Edit/5
        public IActionResult Edit(long id)
        {

            var dinner = _context.Dinners.Find(id);
            var user = _userManager.GetUserName(User);
            if (!dinner.IsUserHost(user))
            {
                return View("~/Views/Dinner/permission.cshtml");
            }

            if (dinner == null)
            {
                return NotFound();
            }
            return View(dinner);
        }

        // POST: Dinner/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(long id, Dinner dinner)
        {
            if (id != dinner.DinnerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                dinner = _repository.UpdateDinner(dinner);
                return RedirectToAction(nameof(Index));
            }

            return View(dinner);
        }

        // GET: Dinner/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dinner = _context.Dinners
                .FirstOrDefault(m => m.DinnerId == id);
            var user = _userManager.GetUserName(User);
            if (!dinner.IsUserHost(user))
            {
                return View("~/Views/Dinner/permission.cshtml");
            }

            if (dinner == null)
            {
                return NotFound();
            }

            return View(dinner);
        }

        // POST: Dinner/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(long id)
        {
            _repository.DeleteDinner(id);
            return RedirectToAction(nameof(Index));
        }

        private bool DinnerExists(long id)
        {
            return _context.Dinners.Any(e => e.DinnerId == id);
        }

        

    }
}
