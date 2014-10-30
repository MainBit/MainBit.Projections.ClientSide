﻿using Orchard.Forms.Services;
using Orchard.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.FilterEditors.Forms
{
    public class NumericVariableFilterFormValidation : FormHandler
    {
        public Localizer T { get; set; }

        public override void Validating(ValidatingContext context) {
            if (context.FormName == NumericVariableFilterForm.FormName) {

                var min = context.ValueProvider.GetValue("Min");
                var max = context.ValueProvider.GetValue("Max");

                var opMin = (NumericOperator)Enum.Parse(typeof(NumericOperator), context.ValueProvider.GetValue("OperatorMin").AttemptedValue);
                var opMax = (NumericOperator)Enum.Parse(typeof(NumericOperator), context.ValueProvider.GetValue("OperatorMax").AttemptedValue);

                if (opMin == NumericOperator.Ignored && opMax == NumericOperator.Ignored)
                {
                    context.ModelState.AddModelError("OperatorMin", T("At least one operator must be different from the {0}", T("Ignored").Text).Text);
                    //context.ModelState.AddModelError("OperatorMax", T("At least one operator must be different from the {0}", T("Ignored").Text).Text);
                }
                if (opMin == NumericOperator.Equals && opMax != NumericOperator.Ignored)
                {
                    context.ModelState.AddModelError("OperatorMax", T("If {0} set to {1} that {2} must set to {3}",
                        T("Operator for lower limit").Text,
                        T("Is equal to").Text,
                        T("Operator for upper limit"),
                        T("Ignored").Text).Text);
                }

                if (!context.ModelState.IsValid)
                {
                    return;
                }

                // validating mandatory values
                if (opMin != NumericOperator.Ignored)
                {
                    if (min == null || String.IsNullOrWhiteSpace(min.AttemptedValue))
                    {
                        context.ModelState.AddModelError("Min", T("The field {0} is required.", T("Range to upper limit").Text).Text);
                    }
                }
                if (opMax != NumericOperator.Ignored)
                {
                    if (max == null || String.IsNullOrWhiteSpace(max.AttemptedValue))
                    {
                        context.ModelState.AddModelError("Max", T("The field {0} is required.", T("Range to lower limit").Text).Text);
                    }
                }
               
                if (!context.ModelState.IsValid) {
                    return;
                }

                decimal output;
                if (opMin != NumericOperator.Ignored)
                {
                    if (!Decimal.TryParse(min.AttemptedValue, out output) && !IsToken(min.AttemptedValue))
                    {
                        context.ModelState.AddModelError("Min", T("The field {0} should contain a valid number", T("Range to upper limit").Text).Text);
                    }
                }
                if (opMax != NumericOperator.Ignored)
                {
                    if (!Decimal.TryParse(max.AttemptedValue, out output) && !IsToken(max.AttemptedValue))
                    {
                        context.ModelState.AddModelError("Max", T("The field {0} should contain a valid number", T("Range to lower limit").Text).Text);
                    }
                }
            }
        }

        private bool IsToken(string value)
        {
            return value.StartsWith("{") && value.EndsWith("}");
        }
    }
}