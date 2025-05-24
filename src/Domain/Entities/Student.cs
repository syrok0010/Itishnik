namespace Itishnik.Domain.Entities;

public class Student : ApplicationUser
{
    private string _educationalProgram = null!;
    private int _educationStartYear;
    private int _groupNumber;
    private readonly HashSet<GradedCourse> _gradedCourses = [];
    
    public Student() : base("DefaultName", "DefaultSurname") {}
    public Student(
        string name,
        string surname,
        string? patronymic,
        string educationalProgram,
        int groupNumber,
        int educationStartYear) : base(name, surname, patronymic)
    {
        EducationStartYear = educationStartYear;
        EducationalProgram = educationalProgram;
        GroupNumber = groupNumber;
    }

    public IEnumerable<GradedCourse> GradedCourses => _gradedCourses;

    public void AddGradedCourse(GradedCourse gradedCourse)
    {
        _gradedCourses.Add(gradedCourse);
    }

    public string EducationalProgram
    {
        get => _educationalProgram;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(EducationalProgram));
            _educationalProgram = value;
        }
    }

    public int EducationStartYear
    {
        get => _educationStartYear;
        set
        {
            if (DateTime.Now.Year - value > 4)
            {
                throw new ArgumentException(
                    $"Интервал между началом обучения и нынешним годом не может превышать 4 года");
            }
            if (value > DateTime.Now.Year)
            {
                throw new ArgumentException($"Год {value} еще не наступил");
            }

            _educationStartYear = value;
        }
    }

    public int GroupNumber
    {
        get => _groupNumber;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, nameof(GroupNumber));
            _groupNumber = value;
        }
    }
}
