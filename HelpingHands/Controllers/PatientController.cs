using HelpingHands.Data;
using HelpingHands.Models;
using HelpingHands.Models.ViewModels;
using HelpingHands.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HelpingHands.Controllers
{
    [Authorize(Roles = "P")]
    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;
        public PatientController(ApplicationDbContext context, UserService userService)
        {
            _userService = userService;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateContract()
        {
            var cities = _context.City.ToList().OrderBy(c => c.Name);

            var userID = _userService.GetLoggedInUserId();

            var Patient = _context.Patient.Where(p => p.userID == userID).FirstOrDefault();

            var PatientID = Patient.PatientID;

            bool assignedExists = _context.CareContract.Any(cc => cc.PatientID == PatientID && (cc.CareStatus.Contains("Assigned") || cc.CareStatus.Contains("New")));


            if (assignedExists)
            {
                TempData["ErrorMessage"] = "You already have an on-going care contract. Cannot create a new contract.";
            }

            ViewBag.Cities = new SelectList(cities, "CityId", "Name");
            return View();
        }

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
        public async Task<IActionResult> SubmitContract (CreateContract model)
        {
            if (ModelState.IsValid)
            {

                var userID= _userService.GetLoggedInUserId();

                var Patient= _context.Patient.Where(p=>p.userID==userID).FirstOrDefault();

                var PatientID = Patient.PatientID;
              
                bool assignedExists = await _context.CareContract.AnyAsync(c => c.CareStatus.Contains("Assigned") && c.PatientID == PatientID);

                if(assignedExists) {

                    TempData["ErrorMessage"] = "You already have an assigne care contract. Cannot create a new contract.";
                    return RedirectToAction("CreateContract","Patient");
                }


                var contract = new CareContract
                {
                    CareStatus = "New",
                    ContractDate = DateTime.Now.Date,
                    AddressLine1 = model.AddressLine1,
                    AddressLine2 = model.AddressLine2,
                    SuburdID = model.SelectedSuburbId,
                    PatientID = PatientID,
                    WoundDescription = model.WoundDescription,
                    NurseID = null,
                    Archived = false,
                };
                _context.CareContract.Add(contract);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");


            }
            return View(model);
        }

        public IActionResult PatientContracts()
        {
            var userID = _userService.GetLoggedInUserId();

            var Patient = _context.Patient.Where(p => p.userID == userID).FirstOrDefault();

            var PatientID = Patient.PatientID;


            var contracts = _context.CareContract.Where(c=>c.PatientID==PatientID && c.Archived==false && c.CareStatus.Contains("New"))
                .Include(c=>c.Nurse)
                .Include(c=>c.Suburb)
                .ThenInclude(c=>c.City)
                .Include(c=>c.Patient)
                .OrderByDescending(c => c.ContractDate)
                .ToList();

   
            return View(contracts);
        }

        public IActionResult PatientVisits()
        {
            var userID = _userService.GetLoggedInUserId();

            var Patient = _context.Patient.Where(p => p.userID == userID).FirstOrDefault();

            var PatientID = Patient.PatientID;

            var contracts = _context.CareContract.Where(c => c.PatientID == PatientID && !c.CareStatus.Contains("Closed")).ToList();

            var visits = new List<CareVisit>();

            foreach (var contract in contracts)
            {
                visits.AddRange(_context.CareVisit.Where(v => v.ContractID == contract.ContractID));
            }

            visits = visits.OrderByDescending(v => v.VisitDate).ToList();

            return View(visits);
        }

    }


}
