using MainBit.Projections.ClientSide.Models;
using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.Storage.Providers
{
    public interface IStorageProvider : IDependency
    {
        string ProviderName { get; }
        IStorage BindStorage(ClientSideProjectionPart part, string filterCategory, string filterType);
    }
}