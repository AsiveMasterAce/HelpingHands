using HelpingHands.Data;
using HelpingHands.Models;
using HelpingHands.Models.ViewModels;
using HelpingHands.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpingHands.Controllers
{
    public class SharedController : Controller
    {
        private readonly UserService _userService;
        private readonly ApplicationDbContext _context;
        public SharedController(UserService userService,ApplicationDbContext context) 
        {
            _userService = userService;
            _context= context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateTimelinePost()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTimelinePost(CreateTimeline post)
        {
            var user =  _userService.GetLoggedInUser;
   
            if (User.IsInRole("O"))
            {
                if (ModelState.IsValid)
                {
                    var _post = new TimelinePost
                    {
                        PostTitle = post.PostTitle,
                        PostContent = post.PostContent,
                        Likes = 0,
                        Comments = new List<PostComment>(),
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.TimelinePost.Add(_post);
                    await _context.SaveChangesAsync();


                    if (User.IsInRole("O"))
                    {
                        return RedirectToAction("MyTimeLine", "OfficeManager");
                    }
                }
                return View("Error");
            }
            return View("Access Denied");
        }
    }
}
