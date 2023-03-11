using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ClaimExample.Models
{
    public class UserProfileViewModel
    {
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Your First Name is required")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Your lastname is required")]
        public string LastName { get; set; }

        [Display(Name = "User name")]
        public string Username { get; set; }
        [Display(Name = "Date Of Birth")]
        [BindProperty, DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        [Required]
        public string PhoneNumber { get; set; }
        [Display(Name = "Profile Picture")]
        public byte[] ProfilePicture { get; set; }
    }
}
