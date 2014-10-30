using System;
using System.Linq;
using Orchard.Forms.Services;
using Orchard.Localization;

namespace MainBit.Projections.ClientSide.FilterEditors.Forms {
    public class BooleanVariableFilterFormValidation : FormHandler
    {
        public Localizer T { get; set; }

        public override void Validating(ValidatingContext context) {
            if (context.FormName == BooleanVariableFilterForm.FormName)
            {

                var value = context.ValueProvider.GetValue("Value");
                var op = context.ValueProvider.GetValue("Operator");
                var opUndef = context.ValueProvider.GetValue("ValueUndefined");

                // validating mandatory values
                if (value == null || String.IsNullOrWhiteSpace(value.AttemptedValue))
                {
                    context.ModelState.AddModelError("Value", T("The field {0} is required.", T("Comparison value").Text).Text);
                }

                if (!context.ModelState.IsValid) {
                    return;
                }

                var allowedValues = new string[] { "True", "False", "Undefined", "true", "false", "undefined" };

                if (!IsToken(value.AttemptedValue) && !allowedValues.Contains(value.AttemptedValue))
                {
                    context.ModelState.AddModelError("Value", T("The field {0} should contain valid value", T("Comparison value").Text).Text);
                }
            }
        }

        private bool IsToken(string value) {
            return value.StartsWith("{") && value.EndsWith("}");
        }
    }
}