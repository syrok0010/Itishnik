namespace Itishnik.Domain.Entities;

public class Teacher(string name, string surname, string? patronymic) : ApplicationUser(name, surname, patronymic)
{
    private readonly HashSet<Course> _courses = [];

    private Teacher() : this("DefaultName", "DefaultSurname", null) {}

    public IEnumerable<Course> Courses => _courses;

    public void AddCourse(Course course) => _courses.Add(course);
}
