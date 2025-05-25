using System.Text;
using System.Text.Encodings.Web;
using Itishnik.Application.Common.Interfaces;
using Itishnik.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Task = System.Threading.Tasks.Task;

namespace Itishnik.Web.Services;

public class EmailResetPasswordService(
    UserManager<ApplicationUser> userManager,
    IEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor,
    LinkGenerator linkGenerator)
    : IResetPasswordService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly LinkGenerator _linkGenerator = linkGenerator;

    public async Task SendResetPasswordEmail(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return;
        
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
            throw new InvalidOperationException("HttpContext is not available. Cannot generate callback URL for email.");

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var callbackUrl = _linkGenerator.GetUriByPage(
            httpContext,
            page: "/Account/ResetPassword",
            handler: null,
            values: new { area = "Identity", code },
            scheme: httpContext.Request.Scheme,
            host: httpContext.Request.Host
        );

        if (string.IsNullOrEmpty(callbackUrl))
            throw new InvalidOperationException("Failed to generate password reset callback URL.");

        await _emailSender.SendEmailAsync(
            email,
            "Восстановление пароля",
            $"Пожалуйста <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>кликните по ссылке</a>, чтобы установить новый пароль.");
    }
}
