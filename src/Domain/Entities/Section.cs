namespace Itishnik.Domain.Entities;

public class Section
{
    private readonly string _name = null!;
    private bool _isPublic;
    private readonly List<Problem> _problems = [];
    private readonly List<int> _weights = [];

    public Section(
        string name,
        Course course, 
        List<Problem> problems,
        List<int> weights,

    bool isPublic=false)
    {
        Name = name;
        Course = course;
        if (problems.Count != weights.Count)
        {
            throw new ArgumentException("Количество заданий должно совпадать с количеством весовых значений");
        }
        for (int problemWeightIndex = 0; problemWeightIndex < problems.Count; problemWeightIndex++)
        {
            _problems.Add(problems[problemWeightIndex]);
            _weights.Add(weights[problemWeightIndex]);
        }
    }
    
    public Guid Id { get; private init; }
    
    public Course Course { get; private init; }
    public Guid CourseId { get; private init; }

    public IEnumerable<Problem> Problems => _problems;

    public void AddProblem(Problem problem) => _problems.Add(problem);
    
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

    public bool IsPublic
    {
        get => _isPublic;
        set
        {
            switch (value)
            {
                case false:
                    _isPublic = value;
                    return;
                case true:
                    if (_problems.Count == 0)
                    {
                        throw new InvalidOperationException("Нельзя публиковать блок без задач");
                    }

                    _isPublic = true;
                    break;
            }
        }
    }
    
    public string? Description { get; private set; }
}
