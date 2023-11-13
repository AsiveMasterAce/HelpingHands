using HelpingHands.Data;
using HelpingHands.Models;
using HelpingHands.Models.Users;
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
        [HttpGet]
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

        #region PreferredSuburbs
        [HttpGet]
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
        [HttpGet]
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


        public IActionResult DeleteSuburb([FromRoute] int Id)
        {
            var user = _userService.GetLoggedInUser();

            var nurse = _context.Nurse.Where(n => n.userID == user.UserID).FirstOrDefault();

            var prefSuburb = _context.PreferredSuburb.
            Include(s=>s.Suburb).
            Include(s=>s.Nurse)
           .FirstOrDefault(s => s.NurseID == nurse.NurseID && s.SuburbID == Id);


            return View(prefSuburb);
        }

        [HttpPost]
        public IActionResult DeletePrefSuburb(PreferredSuburb model)
        {
                var prefSuburb = _context.PreferredSuburb
               .FirstOrDefault(s => s.NurseID == model.NurseID && s.SuburbID == model.SuburbID);

                if (prefSuburb != null)
                {
                    _context.PreferredSuburb.Remove(prefSuburb);
                    _context.SaveChanges();
                }

                return RedirectToAction(nameof(NurseSuburb));
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

        #endregion

        #region NurseContracts
        public IActionResult Contract()
        {
            var user = _userService.GetLoggedInUser();

            var nurse = _context.Nurse.Where(n => n.userID == user.UserID).FirstOrDefault();
            var contracts = _context.CareContract.Where(cc => cc.NurseID == nurse.NurseID && !cc.CareStatus.Contains("Closed"))
                .Include(cc => cc.Patient)
                .Include(cc => cc.Suburb)
                .ThenInclude(c => c.City)
                .OrderByDescending(c => c.ContractDate)
                .ToList();
            return View(contracts);
        }

        public IActionResult ContractVisits([FromRoute] int Id)
        {
            var visits = _context.CareVisit.Where(v => v.ContractID == Id && v.Archived==false)
                .OrderByDescending(v => v.VisitDate)
                .ToList();
            var visitViewModels = visits.Select(v => new VisitViewModel
            {
                VisitId = v.CareVisitID,
                VisitDate = v.VisitDate,
                WoundCondition = v.WoundCondition,
                Notes = v.Notes
            }).ToList();
            return View(visitViewModels);
        }
        public IActionResult NewContracts()
        {
            var user = _userService.GetLoggedInUser();

            var nurse = _context.Nurse.Where(n => n.userID == user.UserID).FirstOrDefault();

            var nursePreferredSuburbs = _context.PreferredSuburb.Where(p => p.NurseID == nurse.NurseID).Select(p => p.SuburbID).ToList();

            var newCareContracts = _context.CareContract.Where(c => c.CareStatus.Contains("New") && nursePreferredSuburbs.Contains(c.Suburb.SuburbID))
                .Include(c => c.Patient)
                .Include(c => c.Suburb)
                .ThenInclude(c => c.City)
                .ToList();

            return View(newCareContracts);
        }

        #endregion

        #region Posts
        public IActionResult MyTimeLine()
        {
            var posts = _context.TimelinePost.Where(p => p.Archived == false).Include(p => p.Comments)
                .ThenInclude(p => p.User).ToList();

            var timeLine = new TimeLineViewModel
            {
                Posts = posts
            };

            return View(timeLine);
        }


        public IActionResult Post([FromRoute] int id)
        {
            var post = _context.TimelinePost.Where(p => p.Id.Equals(id))
                .Include(p => p.Comments)
                .ThenInclude(p => p.User)
                .FirstOrDefault();

            ViewBag.Post = post;

            return View();
        }
        // POST: Comment on Timeline Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CommentOnPost([FromRoute] int id, CommentOnPost model)
        {
            var userID = _userService.GetLoggedInUserId();
            var post = _context.TimelinePost.Where(p => p.Id.Equals(id)).Include(p => p.Comments).FirstOrDefault();

            if (post != null && !post.Archived)
            {
                if (ModelState.IsValid)
                {
                    var comment = new PostComment
                    {
                        UserId = userID,
                        CommentContent = model.CommentContent,
                        Post = post,
                        CreatedAt = DateTime.Now,
                    };
                    _context.Add(comment);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Post", new { id = id });
                }
                return View("Error");
            }
            return NotFound();
        }

        #endregion

        #region Visit
        public IActionResult AddVisit([FromRoute] int Id)
        {
            var contracts = _context.CareContract.Where(cc => cc.ContractID == Id).FirstOrDefault();

            var newContract = new AddVisitViewModel
            {
                contractId = contracts.ContractID,
            };
            
            return View(newContract);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmitVisit(AddVisitViewModel model)
        {

            if(ModelState.IsValid)
            {
                if (!IsValidVisitDate(model.VisitDate))
                {
                    TempData["ErrorMessage"] = "VisitDate must be unique and cannot be a previous date";
                    return RedirectToAction("AddVisit", "Nurse", new { Id = model.contractId });
                }
                else
                {
                    var visit = new CareVisit
                    {
                        VisitDate = model.VisitDate,
                        AxtimateArriveTime = model.AxtimateArriveTime,
                        ContractID = model.contractId,
                        Archived = false,
                    };

                    _context.CareVisit.Add(visit);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Contract", "Nurse");
                }
            }
            return View(model);
        }

        public IActionResult UpdateArriveTime([FromRoute] int Id)
        {
            var careVisit = _context.CareVisit.Where(c => c.CareVisitID == Id).FirstOrDefault();

            var time = new AddArriveTimeViewModel
            {
                CareVisitId = careVisit.CareVisitID,
            };
            return View(time);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SumbitArriveTime(AddArriveTimeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var careVisit = _context.CareVisit.Where(c => c.CareVisitID == model.CareVisitId).FirstOrDefault();
                careVisit.VisitArriveTime = model.VisitArriveTime;
                _context.Update(careVisit);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult UpdateNotes([FromRoute] int Id)
        {
            var careVisit = _context.CareVisit.Where(c => c.CareVisitID == Id).FirstOrDefault();

            var note = new AddNotesTimeViewModel
            {
                CareVisitId = careVisit.CareVisitID,
                WoundCondition=careVisit.WoundCondition,
                Notes=careVisit.Notes,

            };
            return View(note);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> SubmitVisitNotes(AddNotesTimeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var careVisit = _context.CareVisit.Where(c => c.CareVisitID == model.CareVisitId).FirstOrDefault();
                careVisit.Notes = model.Notes;
                careVisit.WoundCondition = model.WoundCondition;

                _context.Update(careVisit);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult DepartTime([FromRoute] int Id)
        {
            var careVisit = _context.CareVisit.Where(c => c.CareVisitID == Id).FirstOrDefault();

            var time = new AddDepartTimeViewModel
            {
                CareVisitId = careVisit.CareVisitID,
            };

            return View(time);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitDepartTime(AddDepartTimeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var careVisit = _context.CareVisit.Where(c => c.CareVisitID == model.CareVisitId).FirstOrDefault();
                careVisit.VisitDepartTime = model.VisitDepartTime;
                _context.CareVisit.Update(careVisit);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult PatientDet([FromRoute] int Id)
        {


            var patient = _context.Patient.Where(p => p.PatientID == Id)
                .Include(p => p.Suburb)
                .Include(p => p.Suburb)
                .ThenInclude(p => p.City)
                .FirstOrDefault();

            var chronicCon = _context.PatientChronicCondition
                .Where(cc => cc.PatientID == patient.PatientID)
                .Select(cc => cc.ChronicCondition.Name)
                .ToList();

            ViewBag.PatientChronic = chronicCon;

            return View(patient);
        }

        public IActionResult TodayVisits()
        {


            var user = _userService.GetLoggedInUser();

            var nurse = _context.Nurse.Where(n => n.userID == user.UserID).FirstOrDefault();

            var careVisits = _context.CareVisit.Where(c => c.CareContract.NurseID == nurse.NurseID && c.VisitDate.Value.Date == DateTime.Now.Date)
            .Include(c => c.CareContract)
            .ThenInclude(c => c.Patient)
            .ThenInclude(c => c.Suburb)
            .ToList();

            ViewBag.CareVisits = careVisits.OrderByDescending(c => c.VisitDate).ToList();
            return View();
        }



        #endregion



        #region AjaxCalls

        [HttpPost("/Nurse/DeleteVisit/{visitId}")]
        public JsonResult DeleteVisit(int visitId)
        {
            var visit= _context.CareVisit.Where(u => u.CareVisitID == visitId).FirstOrDefault();
            
            visit.Archived = true;
            _context.CareVisit.Update(visit);
            _context.SaveChanges();

            return Json(true);
        }
        [HttpPost("/Nurse/CloseContract/{contractId}")]
        public JsonResult CloseContract(int contractId)
        {
            var contract= _context.CareContract.Where(u => u.ContractID == contractId).FirstOrDefault();

            contract.EndDate = DateTime.Now.Date;
            contract.CareStatus = "Closed";
            _context.CareContract.Update(contract);
            _context.SaveChanges();

            return Json(true);
        }
        [HttpPost("/Nurse/TakeContract/{contractId}")]
        public JsonResult TakeContract(int contractId)
        {

            var user = _userService.GetLoggedInUser();

            var nurse = _context.Nurse.Where(n => n.userID == user.UserID).FirstOrDefault();

            var contract = _context.CareContract.Where(u => u.ContractID == contractId).FirstOrDefault();

            contract.NurseID = nurse.NurseID;
            contract.CareStatus = "Assigned";

            _context.CareContract.Update(contract);
            _context.SaveChanges();

            return Json(true);
        }

        #endregion
        #region Validations
        private bool IsPreferredSuburbExists(int nurseId, int suburbId)
        {
            var existingPreferredSuburb = _context.PreferredSuburb
                .FirstOrDefault(s => s.NurseID == nurseId && s.SuburbID == suburbId);

            return existingPreferredSuburb != null;
        }
        private bool IsValidVisitDate(DateTime? visitDate)
        {
            // Check if VisitDate has already been used or is a previous date
            if (_context.CareVisit.Any(v => v.VisitDate.HasValue && v.VisitDate.Value.Date == visitDate.Value.Date) ||
                visitDate.Value.Date < DateTime.Now.Date)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
