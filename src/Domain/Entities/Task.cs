namespace Itishnik.Domain.Entities;

public class Task : BaseAuditableEntity, IOwnedResource
{
    private string _name = null!;
    private bool _isPublic;
    private readonly string _text = null!;
    private readonly HashSet<Tag> _tags = [];
    
    private Task() {}

    public Task(
        string name,
        string text, 
        Teacher teacher,
        Task? previousVersion = null,
        bool publish = false)
    {
        Teacher = teacher;
        Name = name;
        Text = text;
        IsPublic = publish;
        PreviousVersion = previousVersion;
        if (previousVersion is not null) 
            FirstVersion = previousVersion.FirstVersion ?? previousVersion;
    }

    public Teacher Teacher { get; private init; } = null!;
    public Guid TeacherId { get; private init; }

    public Guid? RightSolutionId { get; private init; }
    
    public Task? FirstVersion { get; private init; }
    public Task? PreviousVersion { get; private init; }
    
    public bool IsPublic
    {
        get => _isPublic;
        set
        {
            if (!_isPublic)
            {
                _isPublic = value;
                return;
            }

            if (value)
                throw new InvalidOperationException("Задача уже опубликована");
            throw new InvalidOperationException("Невозможно скрыть опубликованную задачу");
        }
    }
    
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

    public string Text
    {
        get => _text;
        private init
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(Text));
            _text = value;
        }
    }

    public void AddTag(Tag tag) => _tags.Add(tag);

    public void SetTags(params IEnumerable<Tag> tags)
    {
        _tags.Clear();
        foreach (var tag in tags) 
            _tags.Add(tag);
    }
    
    public Guid GetOwnerId() => TeacherId;
}
