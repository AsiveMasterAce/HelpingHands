using HelpingHands.Data;
using HelpingHands.Models;
using HelpingHands.Models.Users;
using HelpingHands.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelpingHands.Services;
using Microsoft.AspNetCore.Identity;

namespace HelpingHands.Controllers
{
    [Authorize(Roles = "O")]

    public class OfficeManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ValidationService _validate;
        private readonly UserService _userService;
        public OfficeManagerController(ILogger<HomeController> logger, ApplicationDbContext context, UserService userService, ValidationService validation)
        {

            _context = context;
            _userService = userService;
            _validate = validation;
        }
        public IActionResult Index()
        {
            var contracts = _context.CareContract.Where(c => c.Archived == false && c.NurseID == null)
                .Include(c => c.Patient)
                .Include(c => c.Suburb)
                .ThenInclude(c => c.City)
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
            var suburbs = _context.Suburb.Where(c => c.Archived == false)
                .Include(c=>c.City)
                .OrderByDescending(c=>c.City.Name)
                .ToList();

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
            var careContracts = _context.CareContract
                .Where(c => c.Archived == false)
                .Include(c => c.Patient)
                .Include(c => c.Nurse)
                .Include(c => c.Suburb)
                .ThenInclude(c => c.City)
                .OrderByDescending(c => c.ContractDate)
                .ToList();
            return View(careContracts);
        }

        public IActionResult AssignContract([FromRoute] int Id)
        {
            var careContract = _context.CareContract.Where(c =>c.ContractID == Id).FirstOrDefault();

            if (careContract == null)
            {
                NotFound();
            }
            var preferredSuburbs = _context.PreferredSuburb.Where(p => p.SuburbID == careContract.SuburdID).ToList();
            var nurses = new List<Nurse>();
            foreach (var preferredSuburb in preferredSuburbs)
            {
                var nurse = _context.Nurse.Where(n => n.NurseID == preferredSuburb.NurseID).FirstOrDefault();
                if (nurse != null)
                {
                    nurses.Add(nurse);
                }
            }

            ViewBag.Nurses = nurses;

            var assignContract = new AssignContract
            {
                CareContractID = careContract.ContractID,
                NurseID = 0,
            };

            return View(assignContract);
    
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignNurse (AssignContract model)
        {
            if (ModelState.IsValid)
            {
                var careContract = _context.CareContract.Where(c => c.ContractID == model.CareContractID).FirstOrDefault();

                if (careContract == null)
                {
                    NotFound();
                }
                careContract.NurseID = model.NurseID;
                careContract.StartDate = DateTime.Now.Date;
                careContract.CareStatus = "Assigned";

                _context.CareContract.Update(careContract);
                await _context.SaveChangesAsync();

                return RedirectToAction("CareContract","OfficeManager");
            }

            return View(model);
        }

        public IActionResult NewContract()
        {
            var careContracts = _context.CareContract
                .Where(c => c.Archived == false && c.CareStatus.Contains("New"))
                .Include(c => c.Patient)
                .Include(c => c.Nurse)
                .Include(c => c.Suburb)
                .ThenInclude(c => c.City)
                .OrderByDescending(c => c.ContractDate)
                .ToList();

            return View(careContracts);
        }

        public IActionResult Patients()
        {
            var users = _context.Users.Where(u => u.Archived == false && u.UserType.Contains("N")).ToList();
            return View(users);
        }

        public IActionResult NurseUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SubmitNurse(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool checkEMail = IsEmailAlreadyInUse(model.Email);

                if (checkEMail == false)
                {
                    var newUser = new UserModel
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        CellNo = model.CellNo,
                        Password = model.Password,
                        UserType = model.UserType,

                    };
                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();

                    int userId = newUser.UserID;
                    if (model.UserType == "N")
                    {
                        var newNurse = new Nurse
                        {

                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email,
                            CellNo = model.CellNo,
                            Password = model.Password,
                            userID = userId,

                        };
                        _context.Nurse.Add(newNurse);
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = $"Error: Email {model.Email} Is already in use";
                    return RedirectToAction("NurseUser", "OfficeManager");
                }
                return RedirectToAction("Patients", "OfficeManager");
            }
            return View(model);
        }

        public IActionResult MyTimeLine()
        {
            var posts = _context.TimelinePost.Where(p => p.Archived == false).Include(p => p.Comments)
                .ThenInclude(p=>p.User).ToList();

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
                .ThenInclude(p=>p.User)
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
        public bool IsEmailAlreadyInUse(string email)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == email);
            return existingUser != null;
        }


        
    }
}
