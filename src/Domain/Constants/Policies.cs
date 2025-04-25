namespace Itishnik.Domain.Constants;

public abstract class Policies
{
    public const string EnrolledStudent = nameof(EnrolledStudent);
    public const string CanPurge = nameof(CanPurge);
    public const string Owner = nameof(Owner);
    public const string OwnerOrAdmin = nameof(OwnerOrAdmin);
}
