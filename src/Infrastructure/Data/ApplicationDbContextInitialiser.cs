using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace Itishnik.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();

        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public ApplicationDbContextInitialiser(
        ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default roles
        var administratorRole = new IdentityRole<Guid>(Roles.Administrator);
        var teacherRole = new IdentityRole<Guid>(Roles.Teacher);
        var studentRole = new IdentityRole<Guid>(Roles.Student);

        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
            await _roleManager.CreateAsync(teacherRole);
            await _roleManager.CreateAsync(studentRole);
        }

        // Default users
        var administrator = new ApplicationUser("Админ", "Админов")
        {
            UserName = "administrator@localhost", Email = "administrator@localhost", EmailConfirmed = true
        };

        var teacher = new Teacher("Иванов", "Иван", "Иванович") 
            { UserName = "teacher@localhost", Email = "teacher@localhost" };

        var student = new Student(
            "Сергеев", 
            "Сергей", 
            "Сергеевич", 
            "Программная инженерия",
            1,
            2022) { UserName = "student@localhost", Email = "student@localhost" };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Administrator1!");
            if (!string.IsNullOrWhiteSpace(administratorRole.Name))
            {
                await _userManager.AddToRolesAsync(administrator, [administratorRole.Name]);
            }

            await _userManager.CreateAsync(teacher, "Teacher1!");
            if (!string.IsNullOrWhiteSpace(teacherRole.Name))
            {
                await _userManager.AddToRolesAsync(teacher, [teacherRole.Name]);
            }

            await _userManager.CreateAsync(student, "Student1!");
            if (!string.IsNullOrWhiteSpace(studentRole.Name))
            {
                await _userManager.AddToRolesAsync(student, [studentRole.Name]);
            }
        }
    }
}
