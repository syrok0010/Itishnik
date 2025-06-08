namespace Itishnik.Domain.Entities;

public class TaskBlockEntry
{
    private TaskBlockEntry()
    {
        
    }
    
    public TaskBlockEntry(TaskBlock taskBlock, Task task, int position, int weight)
    {
        TaskBlockId = taskBlock.Id;
        TaskBlock = taskBlock;
        TaskId = task.Id;
        Task = task;
        Position = position;
        Weight = weight;
    }

    public Guid Id { get; private set; }
    
    public Guid TaskBlockId { get; private set; }
    public TaskBlock TaskBlock { get; private init; } = null!;

    public Guid TaskId { get; private set; }
    public Task Task { get; private init; } = null!;

    public int Position { get; private init; }
    public int Weight { get; internal set; }
}
