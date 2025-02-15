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
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Путь до файла не может быть пустым");
            }

            _path = value;
        }
    }
}
