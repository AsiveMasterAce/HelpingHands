using HelpingHands.Data;
using HelpingHands.Models;
using HelpingHands.Models.Users;
using HelpingHands.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelpingHands.Services;
using Microsoft.AspNetCore.Identity;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout;
using HelpingHands.Helpers;
using Microsoft.Extensions.Hosting;
using System.Drawing.Printing;

namespace HelpingHands.Controllers
{
    [Authorize(Roles = "O")]

    public class OfficeManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ValidationService _validate;
        private readonly UserService _userService;
        private readonly IWebHostEnvironment _hostEnvironment;
        public OfficeManagerController(ILogger<HomeController> logger, ApplicationDbContext context, UserService userService, ValidationService validation, IWebHostEnvironment hostEnvironment)
        {

            _context = context;
            _userService = userService;
            _validate = validation;
            _hostEnvironment = hostEnvironment;
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
        public IActionResult GetSuburbs(string city = null)
        {
            var suburbs = _context.Suburb
                .Where(c => c.Archived == false && (string.IsNullOrEmpty(city) || c.City.Name == city))
                .Include(c => c.City)
                .OrderByDescending(c => c.City.Name)
                .ToList();

            ViewBag.Cities = _context.City.OrderBy(c => c.Name).ToList();

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

        public IActionResult CareContract(string careStatus, DateTime? startDate, DateTime? endDate)
        {
            IQueryable<CareContract> careContracts = _context.CareContract
                .Where(c => c.Archived == false);

            if (!string.IsNullOrEmpty(careStatus) && careStatus != "ALL")
            {
                careContracts = careContracts.Where(c => c.CareStatus == careStatus);
            }

            if (startDate != null && endDate != null)
            {
                careContracts = careContracts.Where(c => c.ContractDate >= startDate && c.ContractDate <= endDate);
            }

            careContracts = careContracts.Include(c => c.Patient)
                .Include(c => c.Nurse)
                .Include(c => c.Suburb)
                .ThenInclude(c => c.City)
                .OrderByDescending(c => c.ContractDate);

            var filteredCareContracts = careContracts.ToList();

            GeneratePDF generatePDF = new GeneratePDF();

            string webRootPath = _hostEnvironment.WebRootPath;
            string filePath = generatePDF.GeneratePdfContract(filteredCareContracts,webRootPath);
            string pdfUrl = Url.Content("~/CareContracts.pdf");
            ViewBag.PdfFilePath = pdfUrl; // Store the file path in a ViewBag variable

            return View(filteredCareContracts);
        }
      

        public IActionResult AssignContract([FromRoute] int Id)
        {
            var careContract = _context.CareContract.Where(c =>c.ContractID == Id).
                Include(c => c.Patient).
                Include(c=>c.Suburb)
               .ThenInclude(c=>c.City)
                .FirstOrDefault();

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


            ViewBag.PatientName = $"{careContract.Patient.FirstName} {careContract.Patient.LastName} ";
            ViewBag.ContractAddress = $"{careContract.AddressLine1}, {careContract.AddressLine2}, {careContract.Suburb.City.Name}, {careContract.Suburb.PostalCode}";
            ViewBag.WoundCondition = $"{careContract.WoundDescription}";
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

        public IActionResult NewContract(DateTime? startDate, DateTime? endDate)
        {
            IQueryable<CareContract> careContracts = _context.CareContract
                .Where(c => c.Archived == false && c.CareStatus.Contains("New"));

            if (startDate != null && endDate != null)
            {
                careContracts = careContracts.Where(c => c.ContractDate >= startDate && c.ContractDate <= endDate);
            }

            careContracts = careContracts.Include(c => c.Patient)
                .Include(c => c.Nurse)
                .Include(c => c.Suburb)
                .ThenInclude(c => c.City)
                .OrderByDescending(c => c.ContractDate);

            var filteredCareContracts = careContracts.ToList();

            return View(filteredCareContracts);
        }
        public IActionResult ClosedContract(DateTime? startDate, DateTime? endDate)
        {
            IQueryable<CareContract> careContracts = _context.CareContract
                .Where(c => c.Archived == false && c.CareStatus.Contains("Closed"));

            if (startDate != null && endDate != null)
            {
                careContracts = careContracts.Where(c => c.ContractDate >= startDate && c.ContractDate <= endDate);
            }

            careContracts = careContracts.Include(c => c.Patient)
                .Include(c => c.Nurse)
                .Include(c => c.Suburb)
                .ThenInclude(c => c.City)
                .OrderByDescending(c => c.ContractDate);

            var filteredCareContracts = careContracts.ToList();

            return View(filteredCareContracts);
        }

        public IActionResult Nurses()
        {
            var users = _context.Users.Where(u => u.Archived == false && u.UserType.Contains("N")).ToList();

            return View(users);
        }

        public IActionResult NurseActivity([FromRoute] int id, DateTime? startDate, DateTime? endDate)
        {
            var nurse = _context.Nurse
                           .Include(n => n.CareContracts)
                               .ThenInclude(cc => cc.CareVisits)
                           .Include(n => n.CareContracts)
                               .ThenInclude(cc => cc.Patient)
                           .Where(n => n.userID.Equals(id))
                           .FirstOrDefault();

            if (startDate.HasValue && endDate.HasValue)
            {
                nurse.CareContracts = nurse.CareContracts
                                           .Where(cc => cc.ContractDate >= startDate && cc.ContractDate <= endDate).
                                           OrderByDescending(cc=>cc.ContractDate)
                                           .ToList();
            }

            foreach (var contract in nurse.CareContracts)
            {
                contract.CareVisits = contract.CareVisits.OrderByDescending(cv => cv.VisitDate).ToList();
            }

            return View(nurse);
        }


        public IActionResult NurseDetails([FromRoute]int Id)
        {
            var nurse = _context.Nurse.Where(n => n.userID.Equals(Id))
            .Include(n => n.PreferredSuburbs).FirstOrDefault();


            var prefferedSuburbs = _context.PreferredSuburb.Where(ps => ps.NurseID == nurse.NurseID).Select(ps => ps.Suburb.Name).ToList();
            ViewBag.prefferedSuburbs = prefferedSuburbs;
            return View(nurse);
   
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
                return RedirectToAction("Nurses", "OfficeManager");
            }
            return View(model);
        }

        public IActionResult Patients()
        {
            var users = _context.Users.Where(u => u.Archived == false && u.UserType.Contains("P")).ToList();

            return View(users);
        
        } 
        public IActionResult PatientCond([FromRoute]int Id)
        {
            var patient = _context.Patient.Where(p => p.userID.Equals(Id)).FirstOrDefault();
            var viewModel = new PatientChronicConditionViewModel
            {
                PatientID = Id,
                FullName= $"{patient.FirstName} {patient.LastName}",
                ChronicConditions = _context.PatientChronicCondition
                 .Where(cc => cc.PatientID.Equals(Id))
                 .Select(cc => cc.ChronicCondition.Name)
                 .ToList()
            };

            return View(viewModel);

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
