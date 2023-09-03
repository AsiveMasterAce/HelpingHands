using HelpingHands.Data;
using HelpingHands.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpingHands.Controllers
{
    public class OfficeManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public OfficeManagerController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
           
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetSuburbs()
        {
            var suburbs = _context.Suburb.Where(c => c.Archived == false).ToList();

            return View(suburbs);
        }
        [HttpGet]
        public async Task<IActionResult> NursesBySuburb([FromRoute] int Id)
        {
            var suburbs= _context.Suburb.Where(s=>s.SuburbID==Id).Include(s=>s.PreferredSuburbs).ThenInclude(s=>s.Nurse).FirstOrDefault();
         
            if (suburbs==null)
            {
                NotFound();
            }
            var suburb = new Suburb
            {
                SuburbID = suburbs.SuburbID,
                PreferredSuburbs=suburbs.PreferredSuburbs,
                Name=suburbs.Name,

            };
            return View(suburb);
        }
    }
}
