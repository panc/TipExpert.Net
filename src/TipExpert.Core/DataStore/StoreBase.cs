using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TipExpert.Core
{
    public class StoreBase<TEntity> : IDisposable
    {
        private readonly string _filePath;
        private readonly Lazy<List<TEntity>> _entities;
        private readonly object _lockObject = new object();

        public StoreBase(IDataStoreConfiguration configuration, string fileName)
        {
            _entities = new Lazy<List<TEntity>>(_ReadFromFile);
            _filePath = Path.Combine(configuration.AppDataPath, fileName);

            if (!Directory.Exists(configuration.AppDataPath))
                Directory.CreateDirectory(configuration.AppDataPath);
        }

        protected List<TEntity> Entities => _entities.Value;

        public Task SaveChangesAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                lock (_lockObject)
                {
                    if (!_entities.IsValueCreated)
                        return; // do not save before the entities are loaded the first time

                    var content = JsonConvert.SerializeObject(Entities);
                    File.WriteAllText(_filePath, content);

                    OnEntitiesSaved(Entities);
                }
            });
        }

        public void Dispose()
        {
        }

        protected virtual void OnEntitiesLoaded(List<TEntity> entities)
        {
        }

        protected virtual void OnEntitiesSaved(List<TEntity> entities)
        {
        }

        private List<TEntity> _ReadFromFile()
        {
            lock (_lockObject)
            {
                if (!File.Exists(_filePath))
                    return new List<TEntity>();

                var content = File.ReadAllText(_filePath);
                var entities = JsonConvert.DeserializeObject<List<TEntity>>(content) ?? new List<TEntity>();

                OnEntitiesLoaded(entities);

                return entities;
            }
        }
    }
}