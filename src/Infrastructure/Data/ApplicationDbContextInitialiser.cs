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
            UserName = "administrator@localhost",
            Email = "administrator@localhost", 
            EmailConfirmed = true
        };

        var teacher = new Teacher("Иван", "Иванов", "Иванович")
        {
            UserName = "teacher@localhost", 
            Email = "teacher@localhost",
            EmailConfirmed = true
        };
        
        var teacher2 = new Teacher("Иван", "Иванов", "Иванович")
        {
            UserName = "teacher2@localhost", 
            Email = "teacher2@localhost",
            EmailConfirmed = true
        };

        var student = new Student(
            "Сергей", 
            "Сергеев", 
            "Сергеевич", 
            "Программная инженерия",
            1,
            2022)
        {
            UserName = "student@localhost",
            Email = "student@localhost",
            EmailConfirmed = true
        };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Admin111!");
            await _userManager.AddToRolesAsync(administrator, [administratorRole.Name!]);
            await _userManager.CreateAsync(teacher, "Teacher1!");
            await _userManager.AddToRolesAsync(teacher, [teacherRole.Name!]);
            await _userManager.CreateAsync(teacher2, "Teacher1!");
            await _userManager.AddToRolesAsync(teacher2, [teacherRole.Name!]);
            await _userManager.CreateAsync(student, "Student1!");
            await _userManager.AddToRolesAsync(student, [studentRole.Name!]);
        }

        if (!await _context.Courses.IgnoreQueryFilters().AnyAsync())
        {
            List<Course> courses = [new(teacher, "Алгосы 1"), new(teacher, "Алгосы 2")];
            foreach (var course in courses)
            {
                var gradedCourse = new GradedCourse(course, student);
                course.AddGradedCourse(gradedCourse);
                student.AddGradedCourse(gradedCourse);
                teacher.AddCourse(course);
                await _context.GradedCourses.AddAsync(gradedCourse);
            }
            var taskBlock = new TaskBlock(
                "Куча",
                courses[0],
                "Ну структура данных такая");
            
            taskBlock.ChangeTimes(
                DateTime.Now.ToUniversalTime().AddDays(7),
                DateTime.Now.ToUniversalTime().AddDays(14),
                new TimeSpan(5, 0, 0));
            courses[0].AddTaskBlock(taskBlock);
            
            if (!await _context.Tasks.IgnoreQueryFilters().AnyAsync())
            {
                var task = new Domain.Entities.Task(
                    "Пирамидальная сортировка",
                    "Отсортировать с помощью кучи массив",
                    "Текст решения",
                    teacher);
                taskBlock.AddTask(task, 10);
                await _context.Tasks.AddAsync(task);
            }
            await _context.Courses.AddRangeAsync(courses);
        }

        await _context.SaveChangesAsync();
    }
}
