using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using mentor.Models;
using Microsoft.AspNetCore.Authorization;

namespace mentor.Controllers;

public class AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) : Controller
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly SignInManager<IdentityUser> _signInManager = signInManager;

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            // Chuyển hướng đến trang chủ hoặc trang khác
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser { UserName = model.Username };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            // Chuyển hướng đến trang chủ hoặc trang khác
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}