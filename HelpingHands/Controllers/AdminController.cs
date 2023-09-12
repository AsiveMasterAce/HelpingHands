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
            return View();
        }
        public IActionResult Users()
        {
            var users=_context.Users.ToList();

            ViewBag.Users = users;

            return View(users);
        }
    }
}
