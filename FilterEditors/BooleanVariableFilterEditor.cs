using System;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Localization;
using MainBit.Projections.ClientSide.FilterEditors.Forms;
using Orchard.Projections.FilterEditors;

namespace MainBit.Projections.ClientSide.FilterEditors {
    public class BooleanVariableFilterEditor : IVariableFilterEditor
    {
        public BooleanVariableFilterEditor()
        {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public bool CanHandle(Type type) {
            return new[] {
                typeof(Boolean),
                typeof(Boolean?)
            }.Contains(type);
        }

        public string FormName {
            get { return BooleanVariableFilterForm.FormName; }
        }

        public Action<IHqlExpressionFactory> Filter(string property, dynamic formState) {
            return BooleanVariableFilterForm.GetFilterPredicate(formState, property);
        }

        public LocalizedString Display(string property, dynamic formState) {
            return BooleanVariableFilterForm.DisplayFilter(property, formState, T);
        }
    }
}