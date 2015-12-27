using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TipExpert.Core
{
    public class UserStore : IDisposable
    {
        private const string FILE_NAME = "user.json";

        private static UserStore _instance;

        public static UserStore Create(string appDataPath)
        {
            if (_instance == null)
                _instance = new UserStore(appDataPath);

            return _instance;
        }


        private readonly string _filePath;
        private readonly Lazy<List<User>> _user;

        private UserStore(string appDataPath)
        {
            _user = new Lazy<List<User>>(_ReadFilmsFromFile);
            _filePath = Path.Combine(appDataPath, FILE_NAME);
        }

        public User[] Films => _user.Value.ToArray();

        public void AddFilm(User user)
        {
            user.Id = Guid.NewGuid();
            _user.Value.Add(user);
        }

        public void RemoveFilm(User user)
        {
            _user.Value.Remove(user);
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

        public void Dispose()
        {
        }
    }
}