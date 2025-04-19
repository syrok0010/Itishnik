namespace Itishnik.Application.Common.Interfaces;

/// <summary>
/// Представляет текущего пользователя приложения.
/// </summary>
public interface IUser
{
    /// <summary>
    /// Идентификатор пользователя. Может быть null, если пользователь не аутентифицирован.
    /// </summary>
    Guid? Id { get; }

    /// <summary>
    /// Возвращает роли, связанные с текущим пользователем.
    /// Возвращает пустую коллекцию, если пользователь не аутентифицирован или не имеет ролей.
    /// </summary>
    IEnumerable<string> Roles { get; }
}
