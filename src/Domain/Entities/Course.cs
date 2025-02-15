namespace Itishnik.Domain.Entities;

public class Course
{
    private string _name = null!;
    private string _description = null!;
    private HashSet<Student> _students = [];
    private HashSet<Section> _sections = [];
    
    public Guid Id { get; private init; }
    
    public Teacher Teacher { get; private set; }
    public Guid TeacherId { get; private set; }

    public Course(
        string name,
        string description,
        Teacher teacher)
    {
        Name = name;
        Description = description;
        Teacher = teacher;
    }

    public IEnumerable<Student> Students => _students;
    public IEnumerable<Section> Sections => _sections;

    public void AddBlock(Section section)
    {
        _sections.Add(section);
    }

    public void AddStudent(Student student)
    {
        _students.Add(student);
    }

    public string Name
    {
        get => _name;
        private set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Название курса не должно быть пустым");
            }

            _name = value;
        }
    }

    public string Description
    {
        get => _description;
        private set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Описание курса не должно быть пустым");
            }

            _description = value;
        }
        
    }
}
