using HelpingHands.Data;
using HelpingHands.Models.Users;
using HelpingHands.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HelpingHands.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RegisterController(ApplicationDbContext context) { 

            _context=context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult PatientRegister()
        {
            var cities = _context.City.ToList();
            var suburbs = _context.Suburb.ToList();

            ViewBag.Cities = new SelectList(cities, "CityId", "Name");
          

            return View();
        }
        public IActionResult GetSuburbs(int cityId)
        {
            var suburbs = _context.Suburb.Where(s => s.CityID == cityId).ToList();

            var selectListItems = suburbs.Select(s => new SelectListItem
            {
                Value = s.SuburbID.ToString(),
                Text = s.Name
            }).OrderBy(s=>s.Text);

            return Json(selectListItems);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitUser(RegisterPatientViewModel model)
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
                        UserType = "P",
                    };
                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();
                    int userId = newUser.UserID;

                    var newPatient = new Patient
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        CellNo = model.CellNo,
                        Password = model.Password,
                        userID = userId,
                        SuburbID =model.SelectedSuburbId,
                    };
                    _context.Patient.Add(newPatient);
                    await _context.SaveChangesAsync();

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, newUser.FirstName + " " + newUser.LastName),
                        new Claim(ClaimTypes.Role, newUser.UserType.Trim()),
                        new Claim(ClaimTypes.NameIdentifier, Convert.ToString(newUser.UserID)),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = $"Error: Email {model.Email} Is already in use";
                    return RedirectToAction("PatientRegister", "Register");
                }
     
            }
            return View(model);
        }
        public bool IsEmailAlreadyInUse(string email)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == email);
            return existingUser != null;
        }

    }
}
