using System;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Forms.Services;
using Orchard.Localization;
using System.Web.Mvc;
using Orchard.Environment.Extensions;
using System.Globalization;
using System.Text;
using Orchard.Environment;
using Orchard.UI.Resources;

namespace MainBit.Projections.ClientSide.FilterEditors.Forms
{
    public class NumericVariableFilterForm : IFormProvider
    {
        public const string FormName = "NumericVariableFilter";
        
        private readonly Work<IResourceManager> _resourceManager;
        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public NumericVariableFilterForm(IShapeFactory shapeFactory, Work<IResourceManager> resourceManager)
        {
            _resourceManager = resourceManager;
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
                            Id: "fieldset-op-min",
                            _OperatorMin: Shape.SelectList(
                                Id: "operator-min", Name: "OperatorMin",
                                Title: T("Operator for lower limit"),
                                Size: 1,
                                Multiple: false
                                )
                            ),
                        _Fs2: Shape.FieldSet(
                            Id: "fieldset-min",
                            _Min: Shape.TextBox(
                                Id: "min", Name: "Min",
                                Title: T("Value for lower limit"),
                                Classes: new[] { "text medium tokenized" }
                                )
                            ),
                        _Fs3: Shape.FieldSet(
                            Id: "fieldset-op-max",
                            _OperatorMax: Shape.SelectList(
                                Id: "operator-max", Name: "OperatorMax",
                                Title: T("Operator for upper limit"),
                                Size: 1,
                                Multiple: false
                                )
                            ),
                        _Fs4: Shape.FieldSet(
                            Id: "fieldset-max",
                            _Max: Shape.TextBox(
                                Id: "max", Name: "Max",
                                Title: T("Value for upper limit"),
                                Classes: new[] { "text medium tokenized" }
                                )
                            )
                    );
                    
                    _resourceManager.Value.Require("script", "jQuery");
                    _resourceManager.Value.Include("script",
                        "~/Modules/MainBit.Projections.ClientSide/Scripts/mainbit-projection-editor-numeric.js",
                        "~/Modules/MainBit.Projections.ClientSide/Scripts/mainbit-projection-editor-numeric.js");

                    f._Fs1._OperatorMin.Add(new SelectListItem { Value = Convert.ToString(NumericOperator.Ignored), Text = T("Ignored").Text });
                    f._Fs1._OperatorMin.Add(new SelectListItem { Value = Convert.ToString(NumericOperator.Equals), Text = T("Is equal to").Text });
                    f._Fs1._OperatorMin.Add(new SelectListItem { Value = Convert.ToString(NumericOperator.GreaterThan), Text = T("Is greater than").Text });
                    f._Fs1._OperatorMin.Add(new SelectListItem { Value = Convert.ToString(NumericOperator.GreaterThanEquals), Text = T("Is greater than or equal to").Text });

                    f._Fs3._OperatorMax.Add(new SelectListItem { Value = Convert.ToString(NumericOperator.Ignored), Text = T("Ignored").Text });
                    f._Fs3._OperatorMax.Add(new SelectListItem { Value = Convert.ToString(NumericOperator.LessThan), Text = T("Is less than").Text });
                    f._Fs3._OperatorMax.Add(new SelectListItem { Value = Convert.ToString(NumericOperator.LessThanEquals), Text = T("Is less than or equal to").Text });

                    return f;
                };

            context.Form(FormName, form);

        }

        public static Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState, string property) {

            var opMin = (NumericOperator)Enum.Parse(typeof(NumericOperator), Convert.ToString(formState.OperatorMin));
            var opMax = (NumericOperator)Enum.Parse(typeof(NumericOperator), Convert.ToString(formState.OperatorMax));

            decimal outer = 0;
            Action<IHqlExpressionFactory> minPredicate = null, maxPredicate = null;
            if (opMin != NumericOperator.Ignored)
            {
                if (formState.Min != null && decimal.TryParse(formState.Min.ToString(), NumberStyles.None, CultureInfo.InvariantCulture, out outer))
                {
                    minPredicate = GetFilterPredicate(opMin, property, outer);
                }
            }
            if (opMax != NumericOperator.Ignored)
            {
                if (formState.Max != null && decimal.TryParse(formState.Max.ToString(), NumberStyles.None, CultureInfo.InvariantCulture, out outer))
                {
                    maxPredicate = GetFilterPredicate(opMax, property, outer);
                }
            }

            if (minPredicate != null && maxPredicate != null)
            {
                return y => y.And(minPredicate, maxPredicate);
            }
            else
            {
                return minPredicate ?? maxPredicate;
            }
        }

        private static Action<IHqlExpressionFactory> GetFilterPredicate(NumericOperator op, string property, decimal value)
        {
            switch (op)
            {
                case NumericOperator.LessThan:
                    return x => x.Lt(property, value);
                case NumericOperator.LessThanEquals:
                    return x => x.Le(property, value);
                case NumericOperator.Equals:
                    return x => x.Eq(property, value);
                case NumericOperator.GreaterThan:
                    return x => x.Gt(property, value);
                case NumericOperator.GreaterThanEquals:
                    return x => x.Ge(property, value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static LocalizedString DisplayFilter(string fieldName, dynamic formState, Localizer T)
        {

            var opMin = (NumericOperator)Enum.Parse(typeof(NumericOperator), Convert.ToString(formState.OperatorMin));
            var opMax = (NumericOperator)Enum.Parse(typeof(NumericOperator), Convert.ToString(formState.OperatorMax));

            string min = Convert.ToString(formState.Min);
            string max = Convert.ToString(formState.Max);

            var displayFilter = new StringBuilder();
            if (opMin != NumericOperator.Ignored)
            {
                displayFilter.Append(min);
                displayFilter.Append(" ");
                displayFilter.Append(GetSign(opMin));
                displayFilter.Append(" ");
            }
            displayFilter.Append(fieldName);
            if (opMax != NumericOperator.Ignored)
            {
                displayFilter.Append(" ");
                displayFilter.Append(GetSign(opMax));
                displayFilter.Append(" ");
                displayFilter.Append(max);
            }

            return new LocalizedString(displayFilter.ToString());
        }

        private static string GetSign(NumericOperator op)
        {

            switch (op)
            {
                case NumericOperator.LessThan:
                    return "<";
                case NumericOperator.LessThanEquals:
                    return "<=";
                case NumericOperator.Equals:
                    return "=";
                case NumericOperator.GreaterThan:
                    return "<"; // ">"; - depends on the position of the compared values
                case NumericOperator.GreaterThanEquals:
                    return "<="; //">="; - depends on the position of the compared values
                default:
                    return "ERROR";
            }
        }
    }

    public enum NumericOperator
    {
        Ignored,
        LessThan,
        LessThanEquals,
        Equals,
        GreaterThan,
        GreaterThanEquals
    }
}