using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Users.Queries.GetUserList;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Users.Commands.InviteTeachers;

public record InviteTeachersCommand(string[] Emails) : IRequest<UserDto[]>;

public class InviteTeachersCommandHandler(
    IIdentityService identityService,
    IMapper mapper,
    IResetPasswordService resetService)
    : IRequestHandler<InviteTeachersCommand, UserDto[]>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly IMapper _mapper = mapper;
    private readonly IResetPasswordService _resetService = resetService;

    public async Task<UserDto[]> Handle(InviteTeachersCommand request, CancellationToken cancellationToken)
    {
        var users = request.Emails.Select(email =>
            new Teacher("Не установлено", "Не установлено", null)
            {
                Email = email, UserName = email, EmailConfirmed = true
            });
        foreach (var teacher in users)
        {
            var result = await _identityService.CreateUserAsync(teacher);
            if (!result.Succeeded)
                continue;

            await _resetService.SendResetPasswordEmail(teacher.Email!);
        }

        return _mapper.Map<UserDto[]>(users);
    }
}
