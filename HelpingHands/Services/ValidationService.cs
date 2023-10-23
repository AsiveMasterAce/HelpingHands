using HelpingHands.Data;

namespace HelpingHands.Services
{
    public class ValidationService
    {

        private readonly ApplicationDbContext _context;

        public ValidationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool IsEmailAlreadyInUse(string email)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == email);
            return existingUser != null;
        }


    }
}
