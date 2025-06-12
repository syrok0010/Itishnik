using Microsoft.AspNetCore.Identity;

namespace Itishnik.Infrastructure
{
    /// <summary>
    /// Локализация ошибок Identity на русский язык.
    /// </summary>
    public class RuErrorDescriber : IdentityErrorDescriber
    {
        //== Общие ошибки ==//
        public override IdentityError DefaultError()
            => new() { Code = nameof(DefaultError), Description = "Произошла неизвестная ошибка." };

        public override IdentityError ConcurrencyFailure()
            => new() { Code = nameof(ConcurrencyFailure), Description = "Ошибка параллелизма, объект был изменен." };
        
        public override IdentityError InvalidToken()
            => new() { Code = nameof(InvalidToken), Description = "Неверный токен." };

        public override IdentityError LoginAlreadyAssociated()
            => new() { Code = nameof(LoginAlreadyAssociated), Description = "Пользователь с таким логином уже существует." };

        public override IdentityError RecoveryCodeRedemptionFailed()
            => new() { Code = nameof(RecoveryCodeRedemptionFailed), Description = "Не удалось использовать код восстановления." };

        //== Пароли ==//
        public override IdentityError PasswordMismatch()
            => new() { Code = nameof(PasswordMismatch), Description = "Неверный пароль." };

        public override IdentityError PasswordRequiresDigit()
            => new() { Code = nameof(PasswordRequiresDigit), Description = "Пароль должен содержать хотя бы одну цифру ('0'-'9')." };
        
        public override IdentityError PasswordRequiresLower()
            => new() { Code = nameof(PasswordRequiresLower), Description = "Пароль должен содержать хотя бы одну строчную букву ('a'-'z')." };
        
        public override IdentityError PasswordRequiresNonAlphanumeric()
            => new() { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Пароль должен содержать хотя бы один неалфавитный символ." };
        
        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
            => new() { Code = nameof(PasswordRequiresUniqueChars), Description = $"Пароль должен содержать не менее {uniqueChars} уникальных символов." };

        public override IdentityError PasswordRequiresUpper()
            => new() { Code = nameof(PasswordRequiresUpper), Description = "Пароль должен содержать хотя бы одну заглавную букву ('A'-'Z')." };
        
        public override IdentityError PasswordTooShort(int length)
            => new() { Code = nameof(PasswordTooShort), Description = $"Пароль должен быть не менее {length} символов." };
        
        //== Пользователи (User) ==//
        public override IdentityError DuplicateEmail(string email)
            => new() { Code = nameof(DuplicateEmail), Description = $"Email '{email}' уже используется." };

        public override IdentityError DuplicateUserName(string userName)
            => new() { Code = nameof(DuplicateUserName), Description = $"Имя пользователя '{userName}' уже занято." };

        public override IdentityError InvalidEmail(string? email)
            => new() { Code = nameof(InvalidEmail), Description = $"Email '{email}' является некорректным." };

        public override IdentityError InvalidUserName(string? userName)
            => new() { Code = nameof(InvalidUserName), Description = $"Имя пользователя '{userName}' недопустимо, оно может содержать только буквы и цифры." };

        public override IdentityError UserAlreadyHasPassword()
            => new() { Code = nameof(UserAlreadyHasPassword), Description = "У пользователя уже установлен пароль." };

        public override IdentityError UserLockoutNotEnabled()
            => new() { Code = nameof(UserLockoutNotEnabled), Description = "Блокировка для этого пользователя не включена." };

        public override IdentityError UserAlreadyInRole(string role)
            => new() { Code = nameof(UserAlreadyInRole), Description = $"Пользователь уже имеет роль '{role}'." };
        
        public override IdentityError UserNotInRole(string role)
            => new() { Code = nameof(UserNotInRole), Description = $"У пользователя нет роли '{role}'." };
        
        //== Роли (Role) ==//
        public override IdentityError DuplicateRoleName(string role)
            => new() { Code = nameof(DuplicateRoleName), Description = $"Роль '{role}' уже существует." };

        public override IdentityError InvalidRoleName(string? role)
            => new() { Code = nameof(InvalidRoleName), Description = $"Имя роли '{role}' является недопустимым." };
    }
}
