using GymManagement.BLL.ViewModels.AccountViewModels;
using GymManagement.Controllers;
using GymManagement.DAL.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager,ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> login(loginViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("Invalid login ", "Invalid Email Or Password");
                return View(model);
            }
            
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if(result.Succeeded)
            {
                _logger.LogInformation($"User {user.Email} logged in successfully.");
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else if(result.IsLockedOut)
            {
                _logger.LogWarning($"User {user.Email} account locked out.");
                ModelState.AddModelError("Account Locked", "Your account has been locked. Please try again later.");
                return View(model);
            }
            else
            {
                ModelState.AddModelError("Invalid login ", "Invalid Email Or Password");
                return View(model);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
           
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(login)); 
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
