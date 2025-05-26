namespace Itishnik.Domain.Entities;

public class Task : BaseAuditableEntity, IOwnedResource
{
    private bool _isPublic;
    private string _referenceSolutionText = null!;
    private readonly HashSet<Tag> _tags = [];
    private readonly HashSet<TaskBlock> _taskBlocks = [];
    
    private Task() {}

    public Task(
        string name,
        string text, 
        string solutionText,
        Teacher teacher,
        Task? previousVersion = null,
        bool publish = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(Name));
        ArgumentException.ThrowIfNullOrWhiteSpace(text, nameof(Text));
        
        Teacher = teacher;
        Name = name;
        Text = text;
        ReferenceSolutionText = solutionText;
        IsPublic = publish;
        PreviousVersion = previousVersion;
        if (previousVersion is not null) 
            FirstVersion = previousVersion.FirstVersion ?? previousVersion;
    }

    public IEnumerable<TaskBlock> TaskBlocks => _taskBlocks;
    public Teacher Teacher { get; private init; } = null!;
    public Guid TeacherId { get; private init; }

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

    public string Name { get; private init; } = null!;

    public string Text { get; private init; } = null!;

    public string ReferenceSolutionText
    {
        get => _referenceSolutionText;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(ReferenceSolutionText));
            _referenceSolutionText = value;
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
