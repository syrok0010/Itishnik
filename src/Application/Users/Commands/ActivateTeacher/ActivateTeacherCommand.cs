using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;
using Task = System.Threading.Tasks.Task;

namespace Itishnik.Application.Users.Commands.ActivateTeacher;

[Authorize(Roles = Roles.Teacher)]
public record ActivateTeacherCommand(string Name, string Surname, string? Patronymic) : IRequest;

public class ActivateTeacherCommandHandler(IApplicationDbContext db, IUser user) : IRequestHandler<ActivateTeacherCommand>
{
    private readonly IApplicationDbContext _db = db;
    private readonly IUser _user = user;

    public async Task Handle(ActivateTeacherCommand request, CancellationToken cancellationToken)
    {
        var student = await _db.Teachers.FirstAsync(s => s.Id == _user.Id, cancellationToken);
        student.Name = request.Name;
        student.Surname = request.Surname;
        student.Patronymic = request.Patronymic;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
