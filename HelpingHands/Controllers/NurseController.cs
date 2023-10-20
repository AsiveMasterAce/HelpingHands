using HelpingHands.Data;
using HelpingHands.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using RestSharp;
using System.Security.Claims;

namespace HelpingHands.Controllers
{
    [Authorize(Roles = "N")]
    public class NurseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;
        public NurseController(ApplicationDbContext dbContext, UserService userService )
        {
            _context=dbContext;
            _userService=userService;

        }
        public IActionResult Index()
        {

            var user = _userService.GetLoggedInUser();

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
