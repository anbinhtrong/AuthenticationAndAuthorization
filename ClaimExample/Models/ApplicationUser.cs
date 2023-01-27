using Microsoft.AspNetCore.Identity;

namespace ClaimExample.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
