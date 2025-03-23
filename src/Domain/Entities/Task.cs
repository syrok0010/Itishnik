namespace Itishnik.Domain.Entities;

public class Task
{
    private string _name = null!;
    private bool _isPublic;
    private readonly string _description = null!;
    private readonly HashSet<Task> _newVersions = [];
    private readonly HashSet<Tag> _tags = [];

    public Task(
        string name,
        string description,
        Solution solution,
        bool publish=false
    )
    {
        Name = name;
        Description = description;
        _isPublic = false;
        if (publish)
        {
            Publish();
        }
    }

    void Publish()
    {
        if (_isPublic)
        {
            throw new InvalidOperationException("Задача уже опубликована");
        }

        _isPublic = true;
    }

    public bool IsPublic() => _isPublic;
    
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
    
    public Guid TaskSolutionId { get; private init; }
    public void AddNewVersion(Task task) => _newVersions.Add(task);
    public void AddTag(Tag tag) => _tags.Add(tag);
    
}
