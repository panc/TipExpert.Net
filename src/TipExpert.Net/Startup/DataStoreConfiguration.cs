using System.IO;
using Microsoft.Extensions.PlatformAbstractions;

namespace TipExpert.Core.Configuration
{
    public class DataStoreConfiguration : IDataStoreConfiguration
    {
        public DataStoreConfiguration(IApplicationEnvironment applicationEnvironment)
        {
            AppDataPath = Path.Combine(applicationEnvironment.ApplicationBasePath, "App_Data");
        }

        public string AppDataPath { get; }
    }
}