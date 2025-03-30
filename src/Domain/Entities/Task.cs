namespace Itishnik.Domain.Entities;

public class Task
{
    private string _name = null!;
    private bool _isPublic;
    private readonly string _description = null!;
    private readonly HashSet<Task> _newVersions = [];
    private readonly HashSet<Tag> _tags = [];
    
    private Task() {}

    public Task(
        string name,
        string description, 
        Teacher teacher,
        bool publish = false)
    {
        Teacher = teacher;
        Name = name;
        Description = description;
        _isPublic = false;
        if (publish)
        {
            IsPublic = true;
        }
    }

    public Teacher Teacher { get; private init; } = null!;
    public Guid TeacherId { get; private init; }

    public Guid? RightSolutionId { get; private init; }
    
    public bool IsPublic
    {
        get => _isPublic;
        set
        {
            _isPublic = _isPublic switch
            {
                true when value => throw new InvalidOperationException("Задача уже опубликована"),
                true when !value => throw new InvalidOperationException("Невозможно скрыть опубликованную задачу"),
                false when !value => throw new InvalidOperationException("Задача не опубликована"),
                _ => true
            };
        }
    }
    
    public Guid Id { get; private init; }

    public IEnumerable<Task> NewVersions => _newVersions;
    public IEnumerable<Tag> Tags => _tags;
    public string Name
    {
        get => _name;
        private set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(Name));
            _name = value;
        }
    }

    public string Description
    {
        get => _description;
        private init
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(Description));
            _description = value;
        }
    }
    public void AddNewVersion(Task task) => _newVersions.Add(task);
    public void AddTag(Tag tag) => _tags.Add(tag);
}
