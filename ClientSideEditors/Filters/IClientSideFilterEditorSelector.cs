using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.ClientSideEditors.Filters
{
    public interface IClientSideFilterEditorSelector : IDependency
    {
        IClientSideFilterEditor GetEditor(string filterEditorFormName, string clientSideType = null);
        IEnumerable<IClientSideFilterEditor> GetEditors(string filterEditorFormName, string clientSideType = null);
    }

    public class ClientSideFilterEditorSelector : IClientSideFilterEditorSelector
    {
        private readonly IEnumerable<IClientSideFilterEditor> _editors;

        public ClientSideFilterEditorSelector(IEnumerable<IClientSideFilterEditor> editors)
        {
            _editors = editors;
        }

        public IClientSideFilterEditor GetEditor(string filterEditorFormName, string clientSideType = null)
        {
            return _editors.FirstOrDefault(e => e.CanHandle(filterEditorFormName, clientSideType));
        }

        public IEnumerable<IClientSideFilterEditor> GetEditors(string filterEditorFormName, string clientSideType = null)
        {
            return _editors.Where(e => e.CanHandle(filterEditorFormName, clientSideType));
        }
    }
}