using Microsoft.AspNetCore.Identity;

namespace Itishnik.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    private string _name = null!;
    private string _surname = null!;
    
    
    public ApplicationUser(string name, string surname, string? patronymic=null)
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
    public string? Patronymic { get; set; }
}
