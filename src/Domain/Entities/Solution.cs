namespace Itishnik.Domain.Entities;

public class Solution
{
    private int _grade;
    private string _text = null!;
    
    public Guid Id { get; init; }
    
    private Solution() {}

    public Solution(Task task, Student student, string text)
    {
        Task = task;
        Student = student;
        Text = text;
    }

    public string Text
    {
        get => _text;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(Text));
            _text = value;
        }
    }
    
    public required Task Task { get; init; }
    public Guid TaskId { get; init; }
    
    public required Student Student { get; init; }
    public required string StudentId { get; init; }

    public int Grade
    {
        get => _grade;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value, nameof(Grade));
            _grade = value;
        }
    }
}
