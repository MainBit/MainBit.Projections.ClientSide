using System;
using System.Linq;
using Orchard.Forms.Services;
using Orchard.Localization;
using Orchard.DisplayManagement;
using System.Web.Mvc;

namespace MainBit.Projections.ClientSide.Providers.SortCriteria
{
    public class VariableSortCriterionFormProvider : IFormProvider {

        public const string FormName = "VariableSortOrder";
        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public VariableSortCriterionFormProvider(IShapeFactory shapeFactory)
        {
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public void Describe(DescribeContext context) {
            Func<IShapeFactory, object> form =
                shape => {

                    var f = Shape.Form(
                        FormName: FormName,
                        _Fs1: Shape.FieldSet(
                            _Sort: Shape.TextBox(
                                Id: "sort", Name: "Sort",
                                Title: T("Initial sort direction"),
                                Description: T("You should use only {0}", "Ascending, Descending or Tokens"),
                                Classes: new[] { "text medium tokenized" }
                                )
                            ),
                        _Fs2: Shape.FieldSet(
                            _SortUndefined: Shape.SelectList(
                                Id: "sortundefined", Name: "SortUndefined",
                                Title: T("Sort direction if initial sort direction is undefined"),
                                Size: 1,
                                Multiple: false
                                )
                            )
                        );

                    f._Fs2._SortUndefined.Add(new SelectListItem { Value = Convert.ToString(SortDirection.None), Text = T("None").Text });
                    f._Fs2._SortUndefined.Add(new SelectListItem { Value = Convert.ToString(SortDirection.Ascending), Text = T("Ascending").Text });
                    f._Fs2._SortUndefined.Add(new SelectListItem { Value = Convert.ToString(SortDirection.Descending), Text = T("Descending").Text });

                    return f;
                };


            context.Form(FormName, form);

        }
    }
    public enum SortDirection
    {
        None = 0,
        Ascending = 1,
        Descending = 2
    }


    public class SortCriterionFormValitator : FormHandler {
        public Localizer T { get; set; }

        public override void Validating(ValidatingContext context) {
            if (context.FormName == VariableSortCriterionFormProvider.FormName)
            {
                var sort = context.ValueProvider.GetValue("Sort");

                if (sort == null || String.IsNullOrWhiteSpace(sort.AttemptedValue))
                {
                    context.ModelState.AddModelError("Sort", T("The field {0} is required.", T("Initial sort direction").Text).Text);
                }

                if (!context.ModelState.IsValid)
                {
                    return;
                }

                var allowedSort = new string[] { "Ascending", "Descending" };
                if (!IsToken(sort.AttemptedValue) && !allowedSort.Contains(sort.AttemptedValue))
                {
                    context.ModelState.AddModelError("Sort", T("The field {0} should contain valid value", T("Initial sort direction").Text).Text);
                }
            }
        }

        private bool IsToken(string value)
        {
            return value.StartsWith("{") && value.EndsWith("}");
        }
    }
}