using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Tasks.Commands.CreateTag;

[Authorize(Roles = Roles.Administrator)]
[Authorize(Roles = Roles.Teacher)]
public record CreateTagCommand(string Text) : IRequest<Tag>;

public class CreateTagCommandHandler(IApplicationDbContext db) : IRequestHandler<CreateTagCommand, Tag>
{
    private readonly IApplicationDbContext _db = db;

    public async Task<Tag> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = new Tag(request.Text);
        await _db.Tags.AddAsync(tag, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return tag;
    }
}
