using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace TipExpert.Core
{
    public interface IUserStore
    {
        Task AddUser(User user);

        Task RemoveUser(User user);

        Task Update(User user);

        Task<User[]> GetAll();

        Task<User> GetById(ObjectId id);

        Task<User> FindUserByEmail(string email, CancellationToken cancellationToken);
    }
}