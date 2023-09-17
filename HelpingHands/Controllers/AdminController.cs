using HelpingHands.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpingHands.Controllers
{
    [Authorize(Roles = "A")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
     
        public AdminController(ApplicationDbContext context)
        {

            _context = context;
        }

        public IActionResult Index()
        {

            var users = _context.Users.ToList();
            var cities = _context.City.ToList();
            var chronicCons = _context.ChronicCondition.ToList();

            
            ViewBag.CountCities = cities.Count();
            ViewBag.CountChronicCon = chronicCons.Count();
            ViewBag.CountUsers =  users.Count(); 
            return View();

        }
        public IActionResult Users()
        {
            var users=_context.Users.ToList();

            ViewBag.Users = users;

            return View(users);
        }

        public IActionResult AddUser()
        {
            return View();
        }
    }
}
