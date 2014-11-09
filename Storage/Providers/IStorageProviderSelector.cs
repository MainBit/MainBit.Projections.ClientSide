using MainBit.Projections.ClientSide.Storage.Providers;
using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.Storage.Providers
{
    public interface IStorageProviderSelector : IDependency
    {
        IStorageProvider GetProvider(string category, string type);
    }

    public class StorageProviderSelector : IStorageProviderSelector
    {
        public const string Storage = "Storage";
        public const string DefaultProviderName = "Infoset";

        private readonly IEnumerable<IStorageProvider> _storageProviders;

        public StorageProviderSelector(IEnumerable<IStorageProvider> storageProviders)
        {
            _storageProviders = storageProviders;
        }

        public IStorageProvider GetProvider(string category, string type)
        {
            return _storageProviders.OrderByDescending(p => p.Priority).FirstOrDefault(p => p.CanHandle(category, type));
        }
    }
}