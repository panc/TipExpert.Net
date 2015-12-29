using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TipExpert.Core
{
    public class LeagueStore : IDisposable, ILeagueStore
    {
        private const string FILE_NAME = "leagues.json";

        private readonly string _filePath;
        private readonly Lazy<List<League>> _leagues;

        public LeagueStore(string appDataPath)
        {
            _leagues = new Lazy<List<League>>(_ReadFromFile);
            _filePath = Path.Combine(appDataPath, FILE_NAME);
        }

        public Task Add(League league)
        {
            return Task.Run(() =>
            {
                league.Id = Guid.NewGuid();
                _leagues.Value.Add(league);
            });
        }

        public Task Remove(League league)
        {
            return Task.Run(() => _leagues.Value.Remove(league));
        }

        public Task<League[]> GetAll()
        {
            return Task.Run(() => _leagues.Value.ToArray());
        }

        public Task<League> GetById(string id)
        {
            return Task.Run(() =>
            {
                return _leagues.Value.FirstOrDefault(x => x.Id.ToString() == id);
            });
        }

        public void Dispose()
        {
        }

        public Task SaveChangesAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                var content = JsonConvert.SerializeObject(_leagues.Value);
                File.WriteAllText(_filePath, content);
            });
        }

        private List<League> _ReadFromFile()
        {
            if (!File.Exists(_filePath))
                return new List<League>();

            var content = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<League>>(content) ?? new List<League>();
        }
    }
}