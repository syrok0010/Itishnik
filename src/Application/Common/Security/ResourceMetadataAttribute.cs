namespace Itishnik.Application.Common.Security;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ResourceMetadataAttribute(string resourceIdPropertyName, Type resourceType) : Attribute
{
    public string ResourceIdPropertyName { get; } = resourceIdPropertyName;
    public Type ResourceType { get; } = resourceType;
}
