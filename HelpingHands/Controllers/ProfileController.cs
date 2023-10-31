﻿using HelpingHands.Data;
using HelpingHands.Models.Users;
using HelpingHands.Models.ViewModels;
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

        public IActionResult PatientProfile()
        {
            var UserID = _userService.GetLoggedInUserId();

            var patient = _context.Patient.Where(p => p.userID == UserID)
                .Include(p => p.Suburb)
                .Include(p=>p.Suburb)
                .ThenInclude(p=>p.City)
                .FirstOrDefault();

            var chronicCon = _context.PatientChronicCondition
                .Where(cc => cc.PatientID == patient.PatientID)
                .Select(cc => cc.ChronicCondition.Name)
                .ToList();

            ViewBag.PatientChronic= chronicCon;

            return View(patient);
        }
        [HttpGet]
        public IActionResult UpdatePatientProfile()
        {
            var UserID = _userService.GetLoggedInUserId();

            var pateint = _context.Patient.Where(p => p.userID == UserID).FirstOrDefault();

            var updatePatient = new UpdatePatientsProfileViewModel
            {
                PatientID=pateint.PatientID,
                FirstName=pateint.FirstName,
                LastName =pateint.LastName,
                Email = pateint.Email,
                CellNo=pateint.CellNo,
                IDNumber=pateint.IDNumber,
                DOB=pateint.DOB,
                EmergencyPerson=pateint.EmergencyPerson,
                EmergencyPersonNo=pateint.EmergencyPersonNo
            };


            return View(updatePatient);
        }

        public async Task<IActionResult> SubmitPatientProfile(UpdatePatientsProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var pateint = _context.Patient.Where(p => p.PatientID == model.PatientID).FirstOrDefault();

                if (pateint == null)
                {
                    return NotFound();
                }

                pateint.FirstName = model.FirstName;
                pateint.LastName = model.LastName;
                pateint.Email = model.Email;
                pateint.CellNo = model.CellNo;
                pateint.IDNumber = model.IDNumber;
                pateint.DOB = model.DOB.Value;
                pateint.EmergencyPerson=model.EmergencyPerson;
                pateint.EmergencyPersonNo= model.EmergencyPersonNo; 

                _context.Patient.Update(pateint);
                await _context.SaveChangesAsync();
                return RedirectToAction("PatientProfile", "Profile");
            }
            return View(model);
        }
        public IActionResult NurseProfile()
        {
            var UserID = _userService.GetLoggedInUserId();

            var nurse = _context.Nurse.Where(n => n.userID == UserID).Include(n => n.PreferredSuburbs).FirstOrDefault();


            var prefferedSuburbs = _context.PreferredSuburb.Where(ps => ps.NurseID == nurse.NurseID).Select(ps => ps.Suburb.Name).ToList();
            ViewBag.prefferedSuburbs = prefferedSuburbs;
            return View(nurse);
        }
        public IActionResult UpdateNurseProfile()
        {
            var userID = _userService.GetLoggedInUserId();

            var nurse = _context.Nurse.Where(n => n.userID == userID).FirstOrDefault();

            var nurseProfile = new UpdateNurseProfileViewModel
            {
                NurseId=nurse.NurseID,
                FirstName=nurse.FirstName, 
                LastName=nurse.LastName,
                Email=nurse.Email,
                CellNo=nurse.CellNo,
                IDNumber=nurse.IDNumber,
                Gender=nurse.Gender,

            };
            return View(nurseProfile);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitNurseProfile(UpdateNurseProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var nurse = _context.Nurse.Where(n => n.NurseID == model.NurseId).FirstOrDefault();
                if (nurse == null)
                {
                    return NotFound();
                }
                nurse.CellNo = model.CellNo;
                nurse.FirstName = model.FirstName;
                nurse.LastName = model.LastName;
                nurse.Email = model.Email;
                nurse.Gender = model.Gender;
                nurse.IDNumber = model.IDNumber;

                _context.Nurse.Update(nurse);
                await _context.SaveChangesAsync();

               return RedirectToAction("NurseProfile", "Profile");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadProfilePicture(IFormFile image)
        {
            var user = _userService.GetLoggedInUser();
            var nurse = _context.Nurse.Where(n => n.userID == user.UserID).FirstOrDefault();

            var fileName = Path.GetFileName(image.FileName);
            if (image != null && image.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/profiles", fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                nurse.ProfilePicUrl = "/profiles/" + fileName;
                _context.Nurse.Update(nurse);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true, url = "/profiles/" + fileName });
        }  

        [HttpPost]
        public async Task<IActionResult> UploadProfilePicturePatient(IFormFile image)
        {
            var UserID = _userService.GetLoggedInUserId();

            var pateint = _context.Patient.Where(p => p.userID == UserID).FirstOrDefault();

            var fileName = Path.GetFileName(image.FileName);
            if (image != null && image.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/profiles", fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                pateint.ProfilePicUrl = "/profiles/" + fileName;
                _context.Patient.Update(pateint);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true, url = "/profiles/" + fileName });
        }  
        
    }
}
