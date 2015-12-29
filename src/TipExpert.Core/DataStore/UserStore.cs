using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TipExpert.Core
{
    public class UserStore : IDisposable
    {
        private const string FILE_NAME = "user.json";

        private readonly string _filePath;
        private readonly Lazy<List<User>> _user;

        public UserStore(string appDataPath)
        {
            _user = new Lazy<List<User>>(_ReadFilmsFromFile);
            _filePath = Path.Combine(appDataPath, FILE_NAME);
        }

        public User[] User => _user.Value.ToArray();

        public void AddUser(User user)
        {
            user.Id = Guid.NewGuid();
            _user.Value.Add(user);
        }

        public void RemoveUser(User user)
        {
            _user.Value.Remove(user);
        }

        public Task<User> FindUserByEmail(string email, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                return _user.Value.FirstOrDefault(x => x.Email == email);

            }, cancellationToken);
        }

        public Task<User> GetUserById(string id)
        {
            return Task.Run(() =>
            {
                return _user.Value.FirstOrDefault(x => x.Id.ToString() == id);
            });
        }

        public void Dispose()
        {
        }

        public Task SaveChangesAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                var content = JsonConvert.SerializeObject(_user.Value);
                File.WriteAllText(_filePath, content);
            });
        }

        private List<User> _ReadFilmsFromFile()
        {
            if (!File.Exists(_filePath))
                return new List<User>();

            var content = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<User>>(content) ?? new List<User>();
        }
    }
}