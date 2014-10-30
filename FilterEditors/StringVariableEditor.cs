using System;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Localization;
using MainBit.Projections.ClientSide.FilterEditors.Forms;

namespace MainBit.Projections.ClientSide.FilterEditors
{
    public class StringVariableFilterEditor : IVariableFilterEditor
    {
        public StringVariableFilterEditor()
        {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public bool CanHandle(Type type) {
            return new[] {
                typeof(char), 
                typeof(string),
            }.Contains(type);
        }

        public string FormName {
            get { return StringVariableFilterForm.FormName; }
        }

        public Action<IHqlExpressionFactory> Filter(string property, dynamic formState) {
            return StringVariableFilterForm.GetFilterPredicate(formState, property);
        }

        public LocalizedString Display(string property, dynamic formState) {
            return StringVariableFilterForm.DisplayFilter(property, formState, T);
        }
    }
}