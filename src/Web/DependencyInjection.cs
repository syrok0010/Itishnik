using Azure.Identity;
using Itishnik.Application.Common.Interfaces;
using Itishnik.Infrastructure.Data;
using Itishnik.Web.Services;
using Microsoft.AspNetCore.Mvc;


namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddScoped<IUser, CurrentUser>();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        builder.Services.AddExceptionHandler<CustomExceptionHandler>();

        builder.Services.AddRazorPages()
            .AddRazorPagesOptions(options =>
            {
                options.Conventions.AuthorizeFolder("/Account");  // Защита /Account
                options.Conventions.AllowAnonymousToPage("/Account/Login");
                options.Conventions.AllowAnonymousToPage("/Account/Logout");
                options.Conventions.AllowAnonymousToPage("/Account/ForgotPassword");
                options.Conventions.AllowAnonymousToPage("/Account/ForgotPasswordConfirmation");
                options.Conventions.AllowAnonymousToPage("/Account/ResetPassword");
                options.Conventions.AllowAnonymousToPage("/Account/ResetPasswordConfirmation");
                options.Conventions.AllowAnonymousToPage("/Account/ConfirmEmail");
            });

        // Customise default API behaviour
        builder.Services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApiDocument((configure, sp) => { configure.Title = "Itishnik API"; });
    }

    public static void AddKeyVaultIfConfigured(this IHostApplicationBuilder builder)
    {
        var keyVaultUri = builder.Configuration["AZURE_KEY_VAULT_ENDPOINT"];
        if (!string.IsNullOrWhiteSpace(keyVaultUri))
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri(keyVaultUri),
                new DefaultAzureCredential());
        }
    }
}
