namespace Itishnik.Domain.Common;

public interface IOwnedResource
{
    Guid GetOwnerId();
}
