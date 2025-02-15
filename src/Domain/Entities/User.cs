using System.Text.RegularExpressions;

namespace Itishnik.Domain.Entities;

public partial class User
{
    private string _name = null!;
    private string _surname = null!;
    private string _patronymic = null!;
    private string _email = null!;
    private string _password = null!;
    
    private User() {}

    protected User(string name, string surname, string patronymic, string email, string password)
    {
        Name = name;
        Surname = surname;
        Patronymic = patronymic;
        Email = email;
        Password = password;
    }
    
    public Guid Id { get; private init; }
    public string Email
    {
        get => _email;
        set
        {
            var mask = EmailRegex();
            if (!mask.IsMatch(value))
            {
                throw new FormatException("Некорректный формат электронной почты");
            }

            _email = value;
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            if (value.Length is < 8 or > 20)
            {
                throw new ArgumentException("Длина пароля должна быть от 8 до 20 символов");
            }

            _password = value;
        }
    }
    
    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Пустое имя");
            }
            if (value.Any(x => !char.IsLetter(x)))
            {
                throw new ArgumentException("Имя должно содержать только буквы");
            }
            
            _name = value;
        }
    }
    
    public string Surname
    {
        get => _surname;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Пустая фамилия");
            }
            if (value.Any(x => !char.IsLetter(x)))
            {
                throw new ArgumentException("Фамилия должна содержать только буквы");
            }
            
            _surname = value;
        }
    }

    public string Patronymic
    {
        get => _patronymic;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                _patronymic = "-";
                return;
            }
            if (value.Any(x => !char.IsLetter(x) && x != ' '))
            {
                throw new ArgumentException("Отчество должно содержать только буквы и пробелы");
            }
            
            _patronymic = value;
        }
    }

    [GeneratedRegex("([a-zA-Z0-9._-]+@[a-zA-Z0-9._-]+\\.[a-zA-Z0-9_-]+)")]
    private static partial Regex EmailRegex(); 
}
