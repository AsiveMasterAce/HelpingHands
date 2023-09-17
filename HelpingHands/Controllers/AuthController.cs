using Microsoft.AspNetCore.Mvc;
using HelpingHands.Data;
using HelpingHands.Models;
using HelpingHands.Models.Users;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace HelpingHands.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        
        public AuthController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
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
                var user = _context.Users.Where(u => u.Archived == false && u.Email==model.Email && u.Password== model.Password).FirstOrDefault();

                if(user!=null)
                {
                  var role = user.UserType.Trim();
       
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.FirstName +" "+user.LastName),
                        new Claim(ClaimTypes.Role,role),
                        new Claim(ClaimTypes.NameIdentifier, Convert.ToString(user.UserID)),

                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties{ IsPersistent = false };
                    
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);


                    return RedirectToAction("Index", "Home");
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
