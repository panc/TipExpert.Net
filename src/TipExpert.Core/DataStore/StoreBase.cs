﻿using System;
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

        public StoreBase(IDataStoreConfiguration configuration, string fileName)
        {
            _entities = new Lazy<List<TEntity>>(_ReadFromFile);
            _filePath = Path.Combine(configuration.AppDataPath, fileName);
        }

        protected List<TEntity> Entities => _entities.Value;

        public Task SaveChangesAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                var content = JsonConvert.SerializeObject(Entities);
                File.WriteAllText(_filePath, content);
            });
        }

        public void Dispose()
        {
        }

        protected virtual void OnEntitiesLoaded(List<TEntity> entities)
        {
        }

        private List<TEntity> _ReadFromFile()
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