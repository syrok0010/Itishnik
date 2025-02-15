namespace Itishnik.Domain.Entities;

public class Teacher(string name, string surname, string patronymic, string email, string password)
    : User(name, surname, patronymic, email, password)
{
    private readonly HashSet<Course> _courses = [];

    public IEnumerable<Course> Courses => _courses;

    public void AddCourse(Course course) => _courses.Add(course);
}
