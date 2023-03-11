using ClaimExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ClaimExample.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Manage()
        {
            var username = _userManager.GetUserName(User);
            if(string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var userProfile = new UserProfileViewModel
            {
                PhoneNumber = user.PhoneNumber??string.Empty,
                Username = user.UserName ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                ProfilePicture = user.ProfilePicture
            };
            return View(userProfile);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(UserProfileViewModel model, IFormFile ProfilePicture)
        {
            if(!ModelState.IsValid)
            {
                model.Username = User.Identity.Name;
                return View(model);
            }
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.DateOfBirth = model.DateOfBirth;
            if(ProfilePicture != null)
            {
                using (var dataStream = new MemoryStream())
                {
                    await ProfilePicture.CopyToAsync(dataStream);
                    user.ProfilePicture = dataStream.ToArray();
                }
            }
            await _userManager.UpdateAsync(user);
            model.ProfilePicture = user.ProfilePicture;
            model.Username = username;
            return View(model);
        }
    }
}
