namespace School.API.Authorization.OwnedResources
{
    public record StudentOwnedResource(int StudentId);
    public record ParentOwnedResource(int ParentId);
    public record PersonOwnedResource(int PersonId);
}