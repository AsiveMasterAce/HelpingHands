using HelpingHands.Data;
using HelpingHands.Models.Users;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HelpingHands.Services
{
    public class UserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        public UserService(IHttpContextAccessor httpContextAccessor,ApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public int GetLoggedInUserId()
        {
            var claimValue = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(claimValue);
        }

        public UserModel GetLoggedInUser()
        {
            int loggedUserId = GetLoggedInUserId();
            return _context.Users.FirstOrDefault(u => u.UserID == loggedUserId);
        }
    }
}
