using ClaimExample.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace ClaimExample.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private const string _authenticateType = "Basic";
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
			_userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
		{
			return View();
		}

		[Authorize]
		public IActionResult Secret()
		{
			var user = HttpContext.User;
			return View();
		}

		public async Task<IActionResult> Authenticate()
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, "AnAnh"),
				new Claim(ClaimTypes.Email, "ananh@test.com")
			};
			var drivingClaims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, "AnAnh"),
				new Claim("DrivingLicense", "B+")
			};
			var userIdentity = new ClaimsIdentity(claims, _authenticateType);
			var licenseIdentity = new ClaimsIdentity(drivingClaims, CookieAuthenticationDefaults.AuthenticationScheme);
			var userPrincipal = new ClaimsPrincipal(new[] { userIdentity, licenseIdentity });
			await HttpContext.SignInAsync(userPrincipal);

			var user = HttpContext.User;
			return RedirectToAction("Index");
		}

		public async Task<IActionResult> LogOut()
		{
			await _signInManager.SignOutAsync();
			//Redirect to home page    
			return RedirectToAction("Index");
		}

		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
			var findUser = await _userManager.FindByEmailAsync(model.Email);
			if (findUser != null)
			{
				var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Secret");
                }
                if (result.IsLockedOut)
                {
                    return RedirectToAction("Index");
                }
                if (result.IsNotAllowed)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }
            return View();
        }

        public async Task<IActionResult> Register()
		{
			return View();
		}

		[HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Secret");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }

        public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

        [HttpGet]
        public ActionResult UserAccessDenied()
        {
            return View();
        }
    }
}