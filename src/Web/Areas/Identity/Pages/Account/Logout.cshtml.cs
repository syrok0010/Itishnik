#nullable disable

using Itishnik.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Itishnik.Web.Areas.Identity.Pages.Account;

public class LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger) : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly ILogger<LogoutModel> _logger = logger;

    public async Task<IActionResult> OnPost(string returnUrl = null)
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out.");
        return returnUrl != null 
            ? LocalRedirect(returnUrl) 
            : RedirectToPage();
    }
}
