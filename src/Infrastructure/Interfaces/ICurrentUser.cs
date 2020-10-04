namespace Domain.Interfaces
{
    public interface ICurrentUser
    {
        string Id { get; }
        string Name { get; }
        string Permissions { get; }
    }
}