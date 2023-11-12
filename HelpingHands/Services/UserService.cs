using HelpingHands.Data;
using HelpingHands.Models.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HelpingHands.Services
{
    public class UserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        public UserService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public int GetLoggedInUserId()
        {
            var claimValue = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(claimValue == null)
            {
                
            }
            return int.Parse(claimValue);
        }

        public UserModel GetLoggedInUser()
        {
            int loggedUserId = GetLoggedInUserId();
            return _context.Users.FirstOrDefault(u => u.UserID == loggedUserId);
        }
        public Patient GetPatient()
        {
            int loggedUserId = GetLoggedInUserId();
            return _context.Patient.FirstOrDefault(p => p.userID == loggedUserId);
        }

        public Nurse GetNurse()
        {
            int loggedUserId = GetLoggedInUserId();
            return _context.Nurse.FirstOrDefault(n => n.userID == loggedUserId);
        }
        public async Task SignInUser(UserModel user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(ClaimTypes.Role, user.UserType.Trim()),
                new Claim(ClaimTypes.NameIdentifier, Convert.ToString(user.UserID)),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
