using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.Storage
{
    public interface IStorage : IDependency
    {
        T Get<T>(string name);
        void Set<T>(string name, T value);
    }
    public static class IStorageExtensions
    {
        public static T Get<T>(this IStorage storage)
        {
            return storage.Get<T>(null);
        }
        public static void Set<T>(this IStorage storage, T value)
        {
            storage.Set(null, value);
        }
    }
}