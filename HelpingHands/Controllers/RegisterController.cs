using HelpingHands.Data;
using HelpingHands.Models.Users;
using HelpingHands.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using HelpingHands.Services;

namespace HelpingHands.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        private readonly ValidationService _validationService;



        public RegisterController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, ValidationService validationService, EncryptService encrypt)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _validationService = validationService;
     
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult PatientRegister()
        {
            var cities = _context.City.ToList().OrderBy(c=>c.Name);

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
        public async Task<IActionResult> RegisterUser(RegisterPatientViewModel model)
        { 
            if (ModelState.IsValid)
            {
               
                bool checkEMail= _validationService.IsEmailAlreadyInUse(model.Email);

                if (checkEMail == false)
                {

                    string hashPassword = EncryptService.HashPassword(model.Password);

                    string trimMail =model.Email.ToLower().Trim();

                    var newUser = new UserModel
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = trimMail,
                        CellNo = model.CellNo,
                        Password = hashPassword,
                        UserType = "P",
                    };
                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();
                    int userId = newUser.UserID;

                    var newPatient = new Patient
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = trimMail,
                        CellNo = model.CellNo,
                        Password = hashPassword,
                        AddressLine1=model.AddressLine1,
                        AddressLine2=model.AddressLine2,
                        userID = userId,
                        SuburbID = model.SelectedSuburbId,
                        ProfilePicUrl=null,
                    };
                    _context.Patient.Add(newPatient);
                    await _context.SaveChangesAsync();


                    var userService = new UserService(_httpContextAccessor, _context);

                    await userService.SignInUser(newUser);

                    
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


    }
}
