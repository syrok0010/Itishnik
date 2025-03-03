namespace Itishnik.Domain.Entities;

public class Section
{
    private readonly string _name = null!;
    private readonly List<Problem> _problems = [];
    private readonly List<int> _weights = [];
    private bool _isPublic;

    public Section(
        string name,
        Course course, 
        List<Problem> problems,
        List<int> weights,
        bool publicate=false
    )
    {
        Name = name;
        Course = course;
        if (problems.Count > 10)
        {
            throw new ArgumentException("Количество заданий не должно превышать 10");
        }
        if (problems.Count != weights.Count)
        {
            throw new ArgumentException("Количество заданий должно совпадать с количеством весовых значений");
        }
        foreach (var problem in problems)
        {
            _problems.Add(problem);
        }
        foreach (var weight in weights)
        {
            _weights.Add(weight);
        }

        if (publicate)
        {
            Publicate();
        }
    }
    
    public Guid Id { get; private init; }
    
    public Course Course { get; private init; }
    public Guid CourseId { get; private init; }

    public IEnumerable<Problem> Problems => _problems;

    public void AddProblem(Problem problem, int weight=-1)
    {
        _problems.Add(problem);
        switch (weight)
        {
            case 0:
                throw new ArgumentException("Вес не должен быть нулевым");
            case -1:
                _weights.Add(10 - _weights.Sum());
                break;
            default:
                _weights.Add(weight);
                break;
        }
    }

    private void Publicate()
    {
        if (_isPublic)
        {
            return;
        }
    
        if (_problems.Count == 0)
        {
            throw new InvalidOperationException("Невозможно опубликовать пустой блок задач");
        }
    
        if (_weights.Sum() > 10)
        {
            throw new InvalidOperationException("Сумма весов должна равняться 10");
        }
    
        _isPublic = true;
    }

    private void Hide()
    {
        if (!_isPublic)
        {
            return;
        }

        _isPublic = false;
    }

    public bool IsPublic
    {
        get => _isPublic;
        set
        {
            if (value)
            {
                Publicate();
            }
            else
            {
                Hide();
            }
        }
    }
    
    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Название блока не может быть пустым");
            }
        }
    }

    public string? Description { get; private set; }
}
