using System;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Projections.Models;
using MainBit.Projections.ClientSide.FilterEditors.Forms;

namespace MainBit.Projections.ClientSide.FieldTypeEditors {
    /// <summary>
    /// <see cref="IFieldTypeEditor"/> implementation for boolean properties
    /// </summary>
    public class BooleanVariableFieldTypeEditor : IVariableFieldTypeEditor
    {
        public Localizer T { get; set; }

        public BooleanVariableFieldTypeEditor()
        {
            T = NullLocalizer.Instance;
        }

        public bool CanHandle(Type storageType) {
            return new[] { typeof(bool), typeof(bool?) }.Contains(storageType);
        }

        public string FormName {
            get { return BooleanVariableFilterForm.FormName; }
        }

        public Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState) {
            return BooleanVariableFilterForm.GetFilterPredicate(formState, "Value");
        }

        public LocalizedString DisplayFilter(string fieldName, string storageName, dynamic formState) {
            return BooleanVariableFilterForm.DisplayFilter(fieldName + " " + storageName, formState, T);
        }

        public Action<IAliasFactory> GetFilterRelationship(string aliasName) {
            return x => x.ContentPartRecord<FieldIndexPartRecord>().Property("IntegerFieldIndexRecords", aliasName);
        }
    }
}