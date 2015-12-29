using System.Threading;
using System.Threading.Tasks;

namespace TipExpert.Core
{
    public interface IUserStore
    {
        Task AddUser(User user);

        Task RemoveUser(User user);

        Task<User[]> GetAll();

        Task<User> GetById(string id);

        Task<User> FindUserByEmail(string email, CancellationToken cancellationToken);

        Task SaveChangesAsync();
    }
}