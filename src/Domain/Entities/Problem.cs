namespace Itishnik.Domain.Entities;

public class Problem
{
    private string _name = null!;
    private readonly string _description = null!;
    private readonly HashSet<Problem> _newVersions = [];
    private readonly HashSet<Tag> _tags = [];

    public Problem(
        string name,
        string description,
        Solution solution,
        bool isPublic
    )
    {
        Name = name;
        Description = description;
        Solution = solution;
        IsPublic = isPublic;
    }
    
    public Guid Id { get; private init; }

    public IEnumerable<Problem> NewVersions => _newVersions;
    public IEnumerable<Tag> Tags => _tags;
    public string Name
    {
        get => _name;
        private set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Название задачи не должно быть пустым");
            }

            _name = value;
        }
    }

    public string Description
    {
        get => _description;
        private init
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Условие задачи не должно быть пустым");
            }

            _description = value;
        }
    }
    
    public bool IsPublic { get; private set; }
    
    public Solution Solution { get; private init; }
    public Guid TaskSolutionId { get; private init; }

    public void AddNewVersion(Problem problem) => _newVersions.Add(problem);
    public void AddTag(Tag tag) => _tags.Add(tag);
    
}
