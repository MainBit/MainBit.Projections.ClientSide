using System;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Localization;
using MainBit.Projections.ClientSide.FilterEditors.Forms;
using Orchard.Projections.FilterEditors;

namespace MainBit.Projections.ClientSide.FilterEditors {
    public class NumericVariableFilterEditor : IVariableFilterEditor
    {
        public NumericVariableFilterEditor()
        {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public bool CanHandle(Type type)
        {
            return new[] {
                typeof(Byte), 
                typeof(SByte), 
                typeof(Int16), 
                typeof(Int32), 
                typeof(Int64), 
                typeof(UInt16), 
                typeof(UInt32), 
                typeof(UInt64), 
                typeof(float), 
                typeof(double), 
                typeof(decimal), 
            }.Contains(type);
        }

        public string FormName {
            get { return NumericVariableFilterForm.FormName; }
        }

        public Action<IHqlExpressionFactory> Filter(string property, dynamic formState) {
            return NumericVariableFilterForm.GetFilterPredicate(formState, property);
        }

        public LocalizedString Display(string property, dynamic formState) {
            return NumericVariableFilterForm.DisplayFilter(property, formState, T);
        }
    }
}