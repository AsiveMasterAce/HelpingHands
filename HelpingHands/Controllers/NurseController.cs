using HelpingHands.Data;
using HelpingHands.Models;
using HelpingHands.Models.ViewModels;
using HelpingHands.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.OpenApi.Validations;
using RestSharp;
using System.Security.Claims;

namespace HelpingHands.Controllers
{
    [Authorize(Roles = "N")]
    public class NurseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;
        public NurseController(ApplicationDbContext dbContext, UserService userService)
        {
            _context = dbContext;
            _userService = userService;

        }
        public IActionResult Index()
        {

            var user = _userService.GetLoggedInUser();

            var nurse = _context.Nurse.Where(n => n.userID == user.UserID).FirstOrDefault();


            var contracts = _context.CareContract.Where(cc => cc.NurseID == nurse.NurseID).Include(cc => cc.Patient).Include(cc => cc.Suburb).ToList();


            var nurseContract = _context.CareContract.Where(cc => cc.NurseID == nurse.NurseID).FirstOrDefault();

            ViewBag.CareCons = contracts.Count();
            ViewBag.CareConsAss = contracts.Where(c => c.CareStatus.Contains("Assigned")).Count();
            ViewBag.CareConsClose = contracts.Where(c => c.CareStatus.Contains("Closed")).Count();
            ViewBag.Contract = contracts.Take(6).Where(c => c.CareStatus.Contains("Assigned")).OrderByDescending(c => c.ContractDate);

            var careVisits = _context.CareVisit.Where(c => c.CareContract.NurseID == nurse.NurseID && c.VisitDate.Value.Date == DateTime.Now.Date)
            .Include(c => c.CareContract)
            .ThenInclude(c => c.Patient)
            .ThenInclude(c => c.Suburb)
            .ToList();

            ViewBag.CareVisits = careVisits.OrderByDescending(c => c.VisitDate).ToList();

            return View();
        }

        public IActionResult NurseSuburb()
        {
            var user = _userService.GetLoggedInUser();

            var nurse = _context.Nurse.Where(n => n.userID == user.UserID).FirstOrDefault();

            var myPrefferedSuburbs = _context.PreferredSuburb.Where(p => p.NurseID == nurse.NurseID)
                .Include(p => p.Nurse)
                .Include(p => p.Suburb)
                .ThenInclude(p => p.City)
                .OrderBy(p => p.Suburb.Name)
                .ToList();
            ViewBag.PrefSub = myPrefferedSuburbs;

            return View();
        }

        public IActionResult PrefSuburb()
        {
            var cities = _context.City.ToList().OrderBy(c => c.Name);

            ViewBag.Cities = new SelectList(cities, "CityId", "Name");

            return View();
        }

        [HttpGet]
        public IActionResult GetSuburbs(int cityId)
        {
            var suburbs = _context.Suburb.Where(s => s.CityID == cityId).ToList();

            var selectListItems = suburbs.Select(s => new SelectListItem
            {
                Value = s.SuburbID.ToString(),
                Text = s.Name
            }).OrderBy(s => s.Text);

            return Json(selectListItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitSuburb(AddPrefferedViewModel model)
        {
            var userId = _userService.GetLoggedInUserId();
            var nurse = _context.Nurse.Where(n => n.userID == userId).FirstOrDefault();

            if (ModelState.IsValid)
            {
                bool validateSuburb = IsPreferredSuburbExists(nurse.NurseID, model.SelectedSuburbId);

                if (validateSuburb)
                {
                    TempData["ErrorMessage"] = "You Already Have this Suburb Selected";
                    return RedirectToAction("PrefSuburb", "Nurse");
                }

                var newSuburb = new PreferredSuburb
                {
                    NurseID = nurse.NurseID,
                    SuburbID = model.SelectedSuburbId
                };

                _context.PreferredSuburb.Add(newSuburb);
                await _context.SaveChangesAsync();

                return RedirectToAction("NurseSuburb");
            }

            return View(model);
        }

        private bool IsPreferredSuburbExists(int nurseId, int suburbId)
        {
            var existingPreferredSuburb = _context.PreferredSuburb
                .FirstOrDefault(s => s.NurseID == nurseId && s.SuburbID == suburbId);

            return existingPreferredSuburb != null;
        }

    }
}
