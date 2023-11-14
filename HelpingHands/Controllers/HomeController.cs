using HelpingHands.Data;
using HelpingHands.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Diagnostics;

namespace HelpingHands.Controllers
{
  
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("A"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (User.IsInRole("O"))
                {
                    return RedirectToAction("Index", "OfficeManager");
                } 
                else if (User.IsInRole("N"))
                {
                    return RedirectToAction("Index", "Nurse");
                }               
                else if (User.IsInRole("P"))
                {
                    return RedirectToAction("Index", "Patient");
                }
            }


            ViewBag.Business = _context.Business.ToList();
            ViewBag.FAQ = _context.FAQs.ToList();
                return View();
        }
        public IActionResult Test()
        {
            var users = _context.Users.Where(u => u.Archived == false).ToList();
      

            return View(users);
        }
       
        public IActionResult FAQ()
        {
            
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}