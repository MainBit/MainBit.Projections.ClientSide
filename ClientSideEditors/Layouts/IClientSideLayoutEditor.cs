using Orchard;
using Orchard.Forms.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.ClientSideEditors.Layouts
{
    public interface IClientSideLayoutEditor : IDependency
    {
        bool CanHandle(string editorFormName);
        IDictionary<string, string> BuildDefaultState(Orchard.Projections.Descriptors.Layout.LayoutDescriptor descriptor, IFormManager _formManager);
    }

    public abstract class ClientSideLayoutEditor : IClientSideLayoutEditor
    {
        public virtual bool CanHandle(string editorFormName) { return false; }
        public virtual IDictionary<string, string> BuildDefaultState(Orchard.Projections.Descriptors.Layout.LayoutDescriptor descriptor, IFormManager _formManager) { return null; }
    }
}