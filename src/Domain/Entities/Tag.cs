namespace Itishnik.Domain.Entities;

public class Tag
{
    
    private Tag() {}
    public Tag(string text)
    {
        Text = text;
    }
    
    public Guid Id { get; private init; }

    public string Text { get; private set; } = null!;
}
