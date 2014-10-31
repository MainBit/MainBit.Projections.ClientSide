using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.ClientSideEditors.Layouts
{
    public interface IClientSideLayoutEditorSelector : IDependency
    {
        IClientSideLayoutEditor GetEditor(string editorFormName, string clientSideType = null);
        IEnumerable<IClientSideLayoutEditor> GetEditors(string editorFormName, string clientSideType = null);
    }

    public class ClientSideLayoutEditorSelector : IClientSideLayoutEditorSelector
    {
        private readonly IEnumerable<IClientSideLayoutEditor> _editors;

        public ClientSideLayoutEditorSelector(IEnumerable<IClientSideLayoutEditor> editors)
        {
            _editors = editors;
        }

        public IClientSideLayoutEditor GetEditor(string editorFormName, string clientSideType = null)
        {
            return _editors.FirstOrDefault(e => e.CanHandle(editorFormName));
        }

        public IEnumerable<IClientSideLayoutEditor> GetEditors(string editorFormName, string clientSideType = null)
        {
            return _editors.Where(e => e.CanHandle(editorFormName));
        }
    }
}