namespace Itishnik.Domain.Entities;

public class TaskBlock
{
    private string _name = null!;
    private readonly List<TaskBlockEntry> _taskEntries = [];
    private readonly HashSet<File> _files = [];
    private bool _isPublic;
    
    private TaskBlock() {}
    
    public TaskBlock(
        string name, 
        Course course,
        string? description = null)
    {
        Name = name;
        Course = course;
        Description = description;
    }
    
    public Guid Id { get; private init; }

    public Course Course { get; private init; } = null!;
    public Guid CourseId { get; private init; }

    public IEnumerable<TaskBlockEntry> TasksEntries => _taskEntries;
    public IEnumerable<File> Files => _files;

    public DateTime? StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public TimeSpan? TimeAllowed { get; private set; }

    public void ChangeTimes(DateTime startTime, DateTime endTime, TimeSpan? timeAllowed)
    {
        if (startTime > endTime)
        {
            throw new ArgumentException("Время начала не должно быть позже времени конца");
        }

        if (timeAllowed == TimeSpan.Zero)
        {
            throw new ArgumentException("Время выполнения не должно быть нулевым");
        }

        StartTime = startTime;
        EndTime = endTime;
        TimeAllowed = timeAllowed;
    }

    public void AddTask(Task task, int weight=0)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(weight, nameof(weight));
        _taskEntries.Add(new TaskBlockEntry(this, task, _taskEntries.Count + 1, weight));
    }
    
    public void RemoveTask(Task task)
    {
        if (IsPublic)
            throw new InvalidOperationException("Нельзя удалить задачу из опубликованного блока");

        var index = _taskEntries.FindIndex(e => e.TaskId == task.Id);
        _taskEntries.RemoveAt(index);
    }

    public void SetWeights(IList<int> values)
    {
        if (IsPublic)
            throw new InvalidOperationException("Нельзя удалить задачу из опубликованного блока");
        if (values.Count != _taskEntries.Count)
            throw new ArgumentException("Количества весов не совпадают");
        if (values.Any(v => v is < 0 or > 10))
            throw new ArgumentException("Один из весов вне допустимого диапазона");

        for (int i = 0; i < _taskEntries.Count; i++)
        {
            _taskEntries[i].Weight = values[i];
        }
    }

    public void AddFile(File file)
    {
        _files.Add(file);
    }

    public bool IsPublic
    {
        get => _isPublic;
        set
        {
            switch (_isPublic)
            {
                case true when value:
                    throw new InvalidOperationException("Блок задач уже опубликован");
                case true when !value:
                    throw new InvalidOperationException("Невозможно скрыть опубликованный блок задач");
                case false when !value:
                    throw new InvalidOperationException("Блок задач не опубликован");
            }

            if (_taskEntries.Count == 0)
            {
                throw new InvalidOperationException("Невозможно опубликовать пустой блок задач");
            }
    
            if (_taskEntries.Select(e => e.Weight).Sum() != 10)
            {
                throw new InvalidOperationException("Сумма весов должна равняться 10");
            }

            if (StartTime is null || EndTime is null || TimeAllowed is null || StartTime < TimeProvider.System.GetLocalNow())
            {
                throw new InvalidOperationException("Некорректно задано время выполнения работы");
            } 
            
            _isPublic = true;
            
            foreach (var gradedCourse in Course.GradedCourses)
                gradedCourse.AddTaskBlock(this);
        }
    }
    
    public string Name
    {
        get => _name;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(Name));
            _name = value;
        }
    }

    public string? Description { get; set; }
}
