namespace Itishnik.Domain.Entities;

public class Tag
{
    public Tag(string topic)
    {
        Topic = topic;
    }
    
    public Guid Id { get; private init; }

    public string Topic { get; private set; }
}
