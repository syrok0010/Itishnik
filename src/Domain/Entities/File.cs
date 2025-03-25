namespace Itishnik.Domain.Entities;

public class File
{
    private string _path = null!;
    
    public Guid Id { get; private init; }

    public File(string path)
    {
        Path = path;
    }

    public string Path
    {
        get => _path;
        set
        {
            var check = new FileInfo(value);
            _path = value;
        }
    }
}
