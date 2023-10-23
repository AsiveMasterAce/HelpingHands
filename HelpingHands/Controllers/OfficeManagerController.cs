using HelpingHands.Data;
using HelpingHands.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpingHands.Controllers
{
    [Authorize(Roles = "O")]

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
            var contracts = _context.CareContract.Where(c => c.Archived == false && c.NurseID == null)
                .Include(c => c.Patient)
                .Include(c => c.Suburb)
                .ThenInclude(s => s.City)
                .ToList();


            var nurse = _context.Nurse.Where(c => c.Archived == false).ToList();
            var careVisits = _context.CareVisit.Where(c => c.Archived == false).ToList();

            ViewBag.NurseCount = nurse.Count();
            ViewBag.ContractCounts = contracts.Count();
            ViewBag.VisitsCount = careVisits.Count();


            ViewBag.Visits = careVisits.Take(6);
            ViewBag.Contract = contracts.Take(6).Where(c => c.CareStatus.Contains("New")).OrderByDescending(c => c.ContractDate);
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
            var suburbs = _context.Suburb.Where(s => s.SuburbID == Id).Include(s => s.PreferredSuburbs).ThenInclude(s => s.Nurse).FirstOrDefault();

            if (suburbs == null)
            {
                NotFound();
            }
            var suburb = new Suburb
            {
                SuburbID = suburbs.SuburbID,
                PreferredSuburbs = suburbs.PreferredSuburbs,
                Name = suburbs.Name,

            };
            return View(suburb);
        }

        public IActionResult Example()
        {
            return View();
        }

        public IActionResult CareContract()
        {
            var careContracts = _context.CareContract.Where(c=>c.Archived==false).ToList();
            return View(careContracts);
        }
    }
}
