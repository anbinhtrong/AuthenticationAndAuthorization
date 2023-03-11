using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ClaimExample.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [Display(Name = "Profile Picture")]
        public byte[] ProfilePicture { get; set; }
    }
}
