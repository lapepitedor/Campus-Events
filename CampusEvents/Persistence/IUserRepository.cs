using Campus_Events.Models;

namespace Campus_Events.Persistence
{
    public interface IUserRepository { 
     
        IEnumerable<User> GetAll();
        User GetSingle(Guid id);
        User Add(User entity);
        void Delete(Guid id);
        User Update(User entity);
        User FindByEmail(string email);
        User FindByLogin(string email, string password);
        User FindByPasswordResetToken(string token);
    }
}
