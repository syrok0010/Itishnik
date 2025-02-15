namespace Itishnik.Domain.Entities;

public class Solution
{
    private int _evaluation;
    
    public Guid Id { get; private init; }

    public Solution(Problem problem, Student student, int evaluation)
    {
        Problem = problem;
        Student = student;
        Evaluation = evaluation;
    }
    
    public Problem Problem { get; private init; }
    public Guid ProblemId { get; private init; }
    
    public Student Student { get; private init; }
    public Guid StudentId { get; private init; }

    public int Evaluation
    {
        get => _evaluation;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("Оценка должна быть неотрицательной");
            }

            _evaluation = value;
        }
    }
}
