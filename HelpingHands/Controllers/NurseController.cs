using HelpingHands.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using System.Security.Claims;

namespace HelpingHands.Controllers
{
    [Authorize(Roles = "N")]
    public class NurseController : Controller
    {
        private readonly ApplicationDbContext _context;
        public NurseController(ApplicationDbContext dbContext)
        {
            _context=dbContext;

        }
        public IActionResult Index()
        {
            string claimValue = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            int loggedUserID = int.Parse(claimValue);

            var user = _context.Users.Where(u => u.UserID==loggedUserID).FirstOrDefault();

            var nurse = _context.Nurse.Where(n => n.userID ==user.UserID).FirstOrDefault();


            var contracts = _context.CareContract.Where(cc => cc.NurseID == nurse.NurseID).Include(cc=>cc.Patient).Include(cc=>cc.Suburb).ToList();

            var nurseContract = _context.CareContract.Where(cc => cc.NurseID == nurse.NurseID).FirstOrDefault();

            var careVisits = _context.CareVisit.Where(c => c.ContractID== nurseContract.ContractID).ToList();

            ViewBag.CareCons= contracts.Count();
            ViewBag.CareConsAss = contracts.Where(c => c.CareStatus.Contains("Assigned")).Count();
            ViewBag.CareConsClose = contracts.Where(c => c.CareStatus.Contains("Closed")).Count();
            ViewBag.Contract = contracts.Take(6).Where(c => c.CareStatus.Contains("Assigned")).OrderByDescending(c => c.ContractDate);
            
            return View();
        }
    }
}
