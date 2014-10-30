using System;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Projections.FilterEditors.Forms;
using Orchard.Projections.Models;
using MainBit.Projections.ClientSide.FilterEditors.Forms;

namespace MainBit.Projections.ClientSide.FieldTypeEditors
{
    /// <summary>
    /// <see cref="IFieldTypeEditor"/> implementation for floating point properties
    /// </summary>
    public class FloatFieldTypeEditor : IVariableFieldTypeEditor
    {
        public Localizer T { get; set; }

        public FloatFieldTypeEditor() {
            T = NullLocalizer.Instance;
        }

        public bool CanHandle(Type storageType) {
            return new[] {
                typeof(float), 
                typeof(double), 
            }.Contains(storageType);
        }

        public string FormName {
            get { return NumericVariableFilterForm.FormName; }
        }

        public Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState) {
            return NumericVariableFilterForm.GetFilterPredicate(formState, "Value");
        }

        public LocalizedString DisplayFilter(string fieldName, string storageName, dynamic formState) {
            return NumericVariableFilterForm.DisplayFilter(fieldName + " " + storageName, formState, T);
        }

        public Action<IAliasFactory> GetFilterRelationship(string aliasName) {
            return x => x.ContentPartRecord<FieldIndexPartRecord>().Property("DoubleFieldIndexRecords", aliasName);
        }
    }
}