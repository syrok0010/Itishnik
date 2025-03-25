namespace Itishnik.Domain.Entities;

public class Solution
{
    private int _grade;
    private string _text = null!;
    
    public Guid Id { get; private init; }

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
    
    public Task Task { get; private init; }
    public Guid ProblemId { get; private init; }
    
    public Student Student { get; private init; }
    public Guid StudentId { get; private init; }

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
