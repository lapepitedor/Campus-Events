namespace Campus_Events.Persistence
{
    public interface IUserRegistration
    {
        bool RegisterUserToEvent(Guid eventId, Guid userId);
        bool UnregisterUserFromEvent(Guid eventId, Guid userId);
        bool IsUserRegistered(Guid eventId, Guid userId); 
    }
}
