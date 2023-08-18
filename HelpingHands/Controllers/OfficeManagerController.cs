using HelpingHands.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpingHands.Controllers
{
    public class OfficeManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OfficeManagerController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
           
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> NursesBySuburb()
        {
            var result = await (from preferredSuburb in _context.preferredSuburb
                                join nurse in _context.Nurse on preferredSuburb.NurseID equals nurse.NurseID
                                join suburb in _context.Suburb on preferredSuburb.SuburbID equals suburb.SuburbID
                                group nurse by suburb.Name into nursesBySuburb
                                select new
                                {
                                    Suburb = nursesBySuburb.Key,
                                    Nurses = nursesBySuburb.Select(n => n.FirstName + " " + n.LastName).ToList()
                                }).ToListAsync();

            return View(result);
        }
    }
}
