using HelpingHands.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            //var nurse = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            //var contracts=_context.CareContract.Where(c=>c.NurseID==nurse)
            return View();
        }
    }
}
