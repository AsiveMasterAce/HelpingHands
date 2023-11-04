using Microsoft.AspNetCore.Mvc;
using HelpingHands.Data;
using HelpingHands.Models;
using HelpingHands.Models.Users;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using HelpingHands.Services;

namespace HelpingHands.Controllers
{
    public class AuthController : Controller
    {
        
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ValidationService _validationService;
        public AuthController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            
            return View();
        }
        
        public IActionResult LogIn()
        {

            return View();

        }
        /// <summary>
        /// Cookie Authentication and user validation
        /// Websites used as refrence:
        /// 
        /// https://www.ezzylearning.net/tutorial/implementing-cookies-authentication-in-asp-net-core
        /// https://www.freecodespot.com/blog/cookie-authentication-in-dotnet-core/
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LogInPost(LogInModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.Where(u => u.Archived == false && u.Email==model.Email).FirstOrDefault();

                if(user!=null)
                {
                    bool isPasswordValid = EncryptService.VerifyPassword(model.Password, user.Password);

                    if(isPasswordValid)
                    {

                        var userService = new UserService(_httpContextAccessor, _context);

                        await userService.SignInUser(user);

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Invalid Password";
                        return RedirectToAction("LogIn", "Auth");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid Credentials Supplied";
                    return RedirectToAction("LogIn", "Auth");
                }

            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
              
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Json(new { success = true });
            //return RedirectToAction("Index", "Home");
        }
        //access denied page from
        // https://www.c-sharpcorner.com/article/authentication-and-claim-based-authorisation-with-asp-net-identity-core/
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
