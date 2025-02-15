namespace Itishnik.Domain.Entities;

public class Student : User
{
    private string _educationalProgram = null!;
    private int _educationStartYear;
    private int _groupNumber;
    private readonly HashSet<EvaluationForCourse> _evaluationsForCourses = [];


    public Student(
        string name,
        string surname,
        string patronymic,
        string educationalProgram,
        int groupNumber,
        int educationStartYear,
        string email,
        string password) : base(name, surname, patronymic, email, password)
    {
        EducationStartYear = educationStartYear;
        EducationalProgram = educationalProgram;
        GroupNumber = groupNumber;
    }

    private IEnumerable<EvaluationForCourse> EvaluationsForCourses => _evaluationsForCourses;

    public void AddCourseEvaluation(EvaluationForCourse evaluationForCourse) =>
        _evaluationsForCourses.Add(evaluationForCourse);
    
    public string EducationalProgram
    {
        get => _educationalProgram;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Пустое название образовательной программы");
            }

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
            if (value <= 0)
            {
                throw new ArgumentException("Номер группы должен быть положительным числом");
            }

            _groupNumber = value;
        }
    }
}
