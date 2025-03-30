namespace Itishnik.Domain.Entities;

public class File
{
    public Guid Id { get; private init; }
    
    public string Path { get; private init; } = null!;
    private File() {}

    public File(string path)
    {
        var check = new FileInfo(path);
        Path = path;
    }
}
