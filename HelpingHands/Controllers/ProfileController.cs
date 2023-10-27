using HelpingHands.Data;
using HelpingHands.Models.Users;
using HelpingHands.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpingHands.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;
        public ProfileController(ApplicationDbContext context, UserService userService)
        {
            _userService = userService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NurseProfile()
        {
            var UserID=_userService.GetLoggedInUserId();

            var nurse = _context.Nurse.Where(n => n.userID == UserID).Include(n=>n.PreferredSuburbs).FirstOrDefault();


            var prefferedSuburbs = _context.PreferredSuburb.Where(ps => ps.NurseID == nurse.NurseID).Select(ps=>ps.Suburb.Name) .ToList();
            ViewBag.prefferedSuburbs=prefferedSuburbs;
            return View(nurse);
        }

        public IActionResult PatientProfile()
        {
            var UserID = _userService.GetLoggedInUserId();

            var patient = _context.Patient.Where(p=>p.userID==UserID)
                .Include(p=>p.Suburb)
                .Include(p=>p.PatientCondition)
                .FirstOrDefault();

            return View(patient);
        }
    }
}
