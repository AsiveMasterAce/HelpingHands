using HelpingHands.Data;
using HelpingHands.Models;
using HelpingHands.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpingHands.Controllers
{
    public class SuburbController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SuburbController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Suburbs()
        {
            var Suburbs = _context.Suburb.Where(s => s.Archived == false).Include(s => s.City).ToList();


            return View(Suburbs);
        }

        public IActionResult AddSuburb()
        {
            var city = _context.City.Where(c => c.Archived == false).ToList();

            ViewBag.Cities = city;
            return View();
        }

        public async Task<IActionResult> SubmitSuburb(AddSuburbViewModel model)
        {
            if (ModelState.IsValid)
            {

                bool checkSuburb = SuburbExistsWithPostalCode(model.PostalCode);

                if (checkSuburb == false)
                {
                    var newSuburb = new Suburb
                    {
                        Name=model.Name,
                        CityID=model.CityID,
                        PostalCode=model.PostalCode,
                    };

                    _context.Suburb.Add(newSuburb);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Suburbs", "Suburb");
                }
                else
                {

                }
            }
            return View(model);
        }
        public IActionResult Cities()
        {
            var city = _context.City.Where(c=>c.Archived==false).ToList();

            return View(city);
        }

        public bool SuburbExistsWithPostalCode(int postalCode)
        {
            return _context.Suburb.Any(s => s.PostalCode == postalCode);
        }

    }
}
