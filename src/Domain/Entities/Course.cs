namespace Itishnik.Domain.Entities;

public class Course
{
    private string _name = null!;
    private string _description = null!;
    private HashSet<Student> _students = [];
    private HashSet<TaskBlock> _taskBlocks = [];
    
    public Guid Id { get; private init; }

    public Teacher Teacher { get; set; }
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
    public IEnumerable<TaskBlock> TaskBlocks => _taskBlocks;

    public void AddTaskBlock(TaskBlock taskBlock)
    {
        _taskBlocks.Add(taskBlock);
    }
    
    public void DeleteTaskBlock(TaskBlock taskBlock)
    {
        if (taskBlock.IsPublic())
        {
            throw new InvalidOperationException("Опубликованный блок задач не может быть удален");
        }

        _taskBlocks.Remove(taskBlock);
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
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(Name));
            if (_students.Count != 0)
            {
                throw new InvalidOperationException("Невозможно поменять название курса, пока есть студенты");
            }
            _name = value;
        }
    }

    public string Description
    {
        get => _description;
        private set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(Description));
            _description = value;
        }
        
    }
}
