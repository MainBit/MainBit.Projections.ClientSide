using MainBit.Projections.ClientSide.Services;
using Orchard.Forms.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.ClientSideEditors.Layouts
{
    public class GridClientSideLayoutEditor : ClientSideLayoutEditor
    {
        public override bool CanHandle(string editorFormName)
        {
            return editorFormName == "GridLayout";
        }

        public override IDictionary<string, string> BuildDefaultState(Orchard.Projections.Descriptors.Layout.LayoutDescriptor descriptor, IFormManager _formManager)
        {
            var dictionary = new Dictionary<string, string>();
            var name = QueryFormHelper.GetName(descriptor.Category, descriptor.Type);

            dictionary.Add("Description", QueryFormHelper.GetDisplayName(descriptor.Name.ToString()));
            dictionary.Add("Display", "0");
            dictionary.Add("DisplayType", "Summary");

            var form = _formManager.Build(descriptor.Form);
            Action<object> process = shape => ClientSideFilterFormHelper.PopulateFromShape(shape, dictionary);
            FormNodesProcessor.ProcessForm(form, process);

            dictionary.Add("ClientSideSwitcher", "true");
            dictionary["ClientSideName"] = "grid";

            return dictionary;
        }
    }
}