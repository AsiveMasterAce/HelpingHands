using Microsoft.AspNetCore.Mvc;
using HelpingHands.Data;

namespace HelpingHands.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        
        public AuthController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        public IActionResult Index()
        {
            
            return View();
        }
        
        public IActionResult LogIn()
        {
            return View();

        }


    }
}
