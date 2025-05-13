namespace Itishnik.Domain.Entities;

public class WeightedTask
{
    private int _weight;
    
    private WeightedTask() {}

    public WeightedTask(TaskBlock taskBlock, Task task, int weight)
    {
        Task = task;
        Weight = weight;
    }
    
    public Task Task { get; private init; } = null!;
    public Guid TaskId { get; private init; }

    public TaskBlock TaskBlock { get; private init; } = null!;
    public Guid TaskBlockId { get; private init; }

    public int Weight
    {
        get => _weight;
        set
        {
            if (value is < 0 or > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Значение веса должно быть от 0 до 10");
            }

            _weight = value;
        }
    }
}
