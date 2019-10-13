using Microsoft.AspNetCore.Identity;

namespace TrickingRoyal.Database
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser(string email)
        {
            UserName = email;
            Email = email;
        }
    }
}
