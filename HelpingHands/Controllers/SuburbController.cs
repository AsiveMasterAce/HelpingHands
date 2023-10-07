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

        [HttpPost]
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
   
        public IActionResult EditSuburb([FromRoute]int id)
        {
            var Suburb = _context.Suburb.Where(s => s.SuburbID == id).Include(s => s.City).FirstOrDefault();

            var city = _context.City.Where(c => c.Archived == false).ToList();

            ViewBag.Cities = city;

            var updateSuburb = new UpdateSuburbVM
            {
                suburbID = Suburb.SuburbID,
                CityID = Suburb.CityID,
                PostalCode = Suburb.PostalCode,
                Name = Suburb.Name,
            };

            return View(updateSuburb);
        }
        public async Task<IActionResult> UpdateSuburb(UpdateSuburbVM model)
        {
            if (ModelState.IsValid)
            {
                var Suburb = _context.Suburb.Where(s => s.SuburbID == model.suburbID).FirstOrDefault();
                if(Suburb == null)
                {
                    return NotFound();
                }

                Suburb.Name = model.Name;
                Suburb.CityID = model.CityID;
                Suburb.PostalCode= model.PostalCode;

                _context.Suburb.Update(Suburb);
                await _context.SaveChangesAsync();
                return RedirectToAction("Suburbs", "Suburb");
            }
            return View(model);
        }
        

        public IActionResult Cities()
        {
            var city = _context.City.Where(c=>c.Archived==false).ToList();

            return View(city);
        }

        public IActionResult AddCity()
        {
           return View();
        }
        public async Task<IActionResult> SubmitCity(AddCityVM model)
        {
            if (ModelState.IsValid) 
            {
                bool cityCheck=CheckCityExist(model.Name);
                if (cityCheck)
                {

                    var city = new City
                    {

                        Name = model.Name,
                        Short = model.Short

                    };
                    _context.City.Add(city);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Cities", "Suburb");
                }
                else
                {

                }
            }

            return View(model);
        }

        public IActionResult EditCity([FromRoute] int id)
        {
            var city = _context.City.Where(c => c.CityId == id).FirstOrDefault();
            if (city == null)
            {
                return NotFound();
            }

            var updateCity = new UpdateCityVM
            {
                CityId = city.CityId,
                Name = city.Name,
                Short = city.Short,
            };

            return View(updateCity);
        }

        public async Task<IActionResult> UpdateCity(UpdateCityVM model)
        {

            if (ModelState.IsValid)
            {
                var city = _context.City.Where(c => c.CityId == model.CityId).FirstOrDefault();


                if (city == null)
                {
                    return NotFound();
                }

                city.Name=model.Name;
                city.Short=model.Short;

                _context.City.Update(city);
                await _context.SaveChangesAsync();
                return RedirectToAction("Cities", "Suburb");
            }
            return View(model);
        }

        [HttpPost("/Suburb/DeleteSuburb/{suburbId}")]
        public JsonResult DeleteSuburb(int suburbId)
        {
            var suburb = _context.Suburb.Where(u => u.SuburbID == suburbId).FirstOrDefault();

            suburb.Archived = true;

            _context.Suburb.Update(suburb);
            _context.SaveChanges();

            return Json(true);
        }

        [HttpPost("/Suburb/DeleteCity/{cityId}")]
        public JsonResult DeleteCity(int cityId)
        {
            var city  = _context.City.Where(u => u.CityId == cityId).FirstOrDefault();

            city.Archived = true;

            _context.City.Update(city);
            _context.SaveChanges();

            return Json(true);
        }

        public bool SuburbExistsWithPostalCode(int postalCode)
        {
            return _context.Suburb.Any(s => s.PostalCode == postalCode);
        }

        public bool CheckCityExist(string city)
        {
            return _context.City.Any(c=>c.Name== city); 
        }
    }
}
