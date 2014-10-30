using System;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Forms.Services;
using Orchard.Localization;
using System.Web.Mvc;
using Orchard.Environment.Extensions;
using System.Globalization;

namespace MainBit.Projections.ClientSide.FilterEditors.Forms
{
    public class BooleanVariableFilterForm : IFormProvider {
        public const string FormName = "BooleanVariableFilter";

        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public BooleanVariableFilterForm(IShapeFactory shapeFactory) {
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public void Describe(DescribeContext context)
        {
            Func<IShapeFactory, object> form =
                shape =>
                {

                    var f = Shape.Form(
                        FormName: FormName,
                        _Fs1: Shape.FieldSet(
                            _Operator: Shape.SelectList(
                                Id: "operator", Name: "Operator",
                                Title: T("Operator"),
                                Size: 1,
                                Multiple: false
                                )
                            ),
                        _Fs2: Shape.FieldSet(
                            Id: "fieldset-value",
                            _СomparisonValue: Shape.TextBox(
                                Id: "value", Name: "Value",
                                Title: T("Comparison Value"),
                                Description: T("You should use only {0}", "True, False, Undefined or Tokens"),
                                Classes: new[] { "text medium tokenized" }
                                )
                            ),
                        _Fs3: Shape.FieldSet(
                            _ValueUndefinedOperator: Shape.SelectList(
                                Id: "valueundefined", Name: "ValueUndefined",
                                Title: T("Allowed values of filtered content if comparision value undefined"),
                                Size: 1,
                                Multiple: false
                                )
                            )
                    );

                    f._Fs1._Operator.Add(new SelectListItem { Value = Convert.ToString(BooleanOperator.Equals), Text = T("Is equal to").Text });
                    f._Fs1._Operator.Add(new SelectListItem { Value = Convert.ToString(BooleanOperator.NotEquals), Text = T("Is not equal to").Text });

                    f._Fs3._ValueUndefinedOperator.Add(new SelectListItem { Value = Convert.ToString(BooleanUndefinedOperator.Any), Text = T("Any").Text });
                    f._Fs3._ValueUndefinedOperator.Add(new SelectListItem { Value = Convert.ToString(BooleanUndefinedOperator.Undefined), Text = T("Undefined").Text });
                    f._Fs3._ValueUndefinedOperator.Add(new SelectListItem { Value = Convert.ToString(BooleanUndefinedOperator.True), Text = T("True").Text });
                    f._Fs3._ValueUndefinedOperator.Add(new SelectListItem { Value = Convert.ToString(BooleanUndefinedOperator.False), Text = T("False").Text });

                    return f;
                };

            context.Form(FormName, form);

        }

        public static Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState, string property) {

            var value = Convert.ToString(formState.Value);
            var op = (BooleanOperator)Enum.Parse(typeof(BooleanOperator), Convert.ToString(formState.Operator));
            var opUndef = (BooleanUndefinedOperator)Enum.Parse(typeof(BooleanUndefinedOperator), Convert.ToString(formState.ValueUndefined));

            if (string.IsNullOrWhiteSpace(value))
            {
                switch (opUndef)
                {
                    case BooleanUndefinedOperator.Any:
                        return null; // throw new NotNeedApplyFilterException(); // return x => x.Or(x1 => x1.IsNotNull(property), x2 => x2.IsNull(property));
                    case BooleanUndefinedOperator.Undefined:
                        return x => x.IsNull(property);
                    case BooleanUndefinedOperator.True:
                        return x => x.Gt(property, (long)0);
                    case BooleanUndefinedOperator.False:
                        return x => x.Eq(property, (long)0);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                bool bComparisonValue = false;
                if (value == "undefined" || value == "Undefined" || value == null)
                {
                    return x => x.IsNull(property);
                }

                if (!Boolean.TryParse((string)value, out bComparisonValue))
                {
                    decimal dComparisonValue;
                    if (Decimal.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out dComparisonValue))
                    {
                        bComparisonValue = Convert.ToBoolean(dComparisonValue);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }

                if ((bComparisonValue && op == BooleanOperator.Equals) || (!bComparisonValue && op == BooleanOperator.NotEquals))
                {
                    return x => x.Gt(property, (long)0);
                }
                else
                {
                    return x => x.Eq(property, (long)0);
                }
            }
        }

        public static LocalizedString DisplayFilter(string fieldName, dynamic formState, Localizer T)
        {

            var value = Convert.ToString(formState.Value);
            var op = (BooleanOperator)Enum.Parse(typeof(BooleanOperator), Convert.ToString(formState.Operator));
            var opUndef = (BooleanUndefinedOperator)Enum.Parse(typeof(BooleanUndefinedOperator), Convert.ToString(formState.ValueUndefined));

            string display;
            if (op == BooleanOperator.Equals)
            {
                display = "{0} equals {1}";
            }
            else
            {
                display = "{0} is not equal to {1}";
            }

            switch (opUndef)
            {
                case BooleanUndefinedOperator.Any:
                    display += ", select any if undefined";
                    break;
                case BooleanUndefinedOperator.Undefined:
                    display += ", select undefined if undefined";
                    break;
                case BooleanUndefinedOperator.True:
                    display += ", select true if undefined";
                    break;
                case BooleanUndefinedOperator.False:
                    display += ", select false if undefined";
                    break;
                default:
                    break;
            }

            return T(display, fieldName, value);
        }
    }

    public enum BooleanOperator
    {
        Equals,
        NotEquals
    }

    public enum BooleanUndefinedOperator
    {
        Any,
        Undefined,
        True,
        False
    }
}