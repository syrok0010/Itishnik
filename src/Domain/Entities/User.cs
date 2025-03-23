using Microsoft.AspNetCore.Identity;

namespace Itishnik.Infrastructure.Identity;

public class User : IdentityUser
{
    private string _name = null!;
    private string _surname = null!;
    private string _patronymic = null!;
    
    
    protected User(string name, string surname, string patronymic)
    {
        Name = name;
        Surname = surname;
        Patronymic = patronymic;
    }
    
    public string Name
    {
        get => _name;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(Name));
            _name = value;
        }
    }
    
    public string Surname
    {
        get => _surname;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(Surname));
            _surname = value;
        }
    }
    public string Patronymic
    {
        get => _patronymic;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(Patronymic));
            _patronymic = value;
        }
    }
}
