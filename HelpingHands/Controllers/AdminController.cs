using HelpingHands.Data;
using HelpingHands.Models.ViewModels;
using HelpingHands.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using HelpingHands.Services;

namespace HelpingHands.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {

            _context = context;
        }
        public IActionResult Index()
        {

            var users = _context.Users.ToList();
            var cities = _context.City.ToList();
            var chronicCons = _context.ChronicCondition.ToList();


            ViewBag.CountCities = cities.Count();
            ViewBag.CountChronicCon = chronicCons.Count();
            ViewBag.CountUsers = users.Count();

            ViewBag.Users = users.Where(u => u.Archived == false).Take(6);
   
            return View();

        }

 
        public IActionResult Users([FromQuery] string userType)
        {
            var users = _context.Users.Where(u => u.Archived == false);

            if (!string.IsNullOrEmpty(userType) && userType != "ALL")
            {
                users = users.Where(u => u.UserType.Contains(userType));
            }
       
            var usersList = users.ToList();
            return View(usersList);
        }
        
        public IActionResult AddUser()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitUser(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool checkEMail = IsEmailAlreadyInUse(model.Email);

                if (checkEMail == false)
                {
                    string hashPassword = EncryptService.HashPassword(model.Password);

                    var newUser = new UserModel
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        CellNo = model.CellNo,
                        Password = hashPassword,
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
                            Password = hashPassword,
                            userID = userId,
                            
                        };
                        _context.Nurse.Add(newNurse);
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = $"Error: Email {model.Email} Is already in use";
                    return RedirectToAction("AddUser", "Admin");
                }
                return RedirectToAction("Users", "Admin");
            } 
            return View(model);
        }


        public IActionResult UpdateUserPassword([FromRoute] int Id)
        {
            var user = _context.Users.Where(u => u.UserID == Id).FirstOrDefault();
            var role = user.UserType.Trim();

            var roleName = "";

            if (role == "O")
            {
                roleName = "Office Manager";
            }
            else if (role == "P")
            {
                roleName = "Patient";
            }
            else if (role == "N")
            {
                roleName = "Nurse";
            }
            else if (role == "A")
            {
                roleName = "Admin";
            }

            ViewBag.RoleName = roleName;
            var editUser = new UpdateUserPasswordViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserType = user.UserType,
                UserID = user.UserID,
                Email = user.Email,
                CellNo = user.CellNo,
           
                
            };
            return View(editUser);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPassword(UpdateUserPasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                bool checkPassword = IsPasswordExist(model.UserID, model.Password);

                if (checkPassword == false)
                {
                    var user = _context.Users.Where(u => u.UserID == model.UserID).FirstOrDefault();
                    if (user == null)
                    {
                        return NotFound();
                    }
                    user.Password = EncryptService.HashPassword(model.Password);

                    _context.Users.Update(user);
                    var role = model.UserType.Trim();

                    if (role == "N")
                    {
                        var nurse = _context.Nurse.Where(n => n.userID == model.UserID).FirstOrDefault();
                        nurse.Password = EncryptService.HashPassword(model.Password);
                        _context.Nurse.Update(nurse);
                    }
                    else if (role == "P")
                    {
                        var patient = _context.Patient.Where(p => p.userID == model.UserID).FirstOrDefault();

                        patient.Password = EncryptService.HashPassword(model.Password);
                        _context.Patient.Update(patient);
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Users", "Admin");
                }
                else
                {
                    TempData["ErrorMessage"] = "This password is currently in use";
                    return RedirectToAction("UpdateUserPassword", "Admin");
                }
            }

            return View(model);
        }
        public IActionResult EditUser([FromRoute] int Id)
        {
            var user = _context.Users.Where(u => u.UserID == Id).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }
            var role = user.UserType.Trim();

            var roleName = "";

            if (role == "O")
            {
                roleName = "Office Manager";
            }
            else if (role == "P")
            {
                roleName = "Patient";
            }
            else if (role == "N")
            {
                roleName = "Nurse";
            }
            else if (role == "A")
            {
                roleName = "Admin";
            }

            ViewBag.RoleName = roleName;
            var editUser = new EditUserViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserType = user.UserType,
                UserID = user.UserID,
                Email = user.Email,
                CellNo = user.CellNo
            };

            return View(editUser);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.Where(u => u.UserID == model.UserID).FirstOrDefault();

                if (user == null)
                {
                    return NotFound();
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.CellNo = model.CellNo;
                 
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                var role = model.UserType.Trim();

                if (role=="N")
                {
                    var nurse = _context.Nurse.Where(n => n.userID == model.UserID).FirstOrDefault();
                    nurse.FirstName = model.FirstName;
                    nurse.LastName = model.LastName;
                    nurse.Email = model.Email;
                    nurse.CellNo = model.CellNo;

                    _context.Nurse.Update(nurse);
                    await _context.SaveChangesAsync();
                }
                else if (role =="P")
                {
                    var patient = _context.Patient.Where(p => p.userID == model.UserID).FirstOrDefault();

                    patient.FirstName = model.FirstName;
                    patient.LastName = model.LastName;
                    patient.Email = model.Email;
                    patient.CellNo = model.CellNo;

                    _context.Patient.Update(patient);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Users", "Admin");
            }

             return View(model);
        }

        public IActionResult Business()
        {
            var business = _context.Business.ToList();
           
            ViewBag.Business = business;    

            return View();

        }

        public IActionResult RemovedUsers([FromQuery] string userType)
        {
            var users = _context.Users.Where(u => u.Archived == true);

            if (!string.IsNullOrEmpty(userType) && userType != "ALL")
            {
                users = users.Where(u => u.UserType.Contains(userType));
            }

            var usersList = users.ToList();

            return View(usersList);
        }


        public IActionResult AddBusinessData()
        {
            return View();
        }

        [HttpPost("/Admin/DeleteUser/{userId}")]
        public JsonResult DeleteUser(int userId)
        {
            var user = _context.Users.Where(u => u.UserID ==userId).FirstOrDefault();

            user.Archived = true;

            _context.Users.Update(user);
            _context.SaveChanges();
            
            return Json(true);
        }
        public bool IsEmailAlreadyInUse(string email)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == email);
            return existingUser != null;
        }

        public bool IsPasswordExist(int userId, string password)
        {

            var existingUser = _context.Users.FirstOrDefault(u => u.UserID == userId && u.Password == password);
            return existingUser != null;
        }


    }
}
