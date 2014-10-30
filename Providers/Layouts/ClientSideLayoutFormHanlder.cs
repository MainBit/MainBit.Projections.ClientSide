using MainBit.Projections.ClientSide.ClientSideEditors.SortCriteria;
using MainBit.Projections.ClientSide.Services;
using Orchard.DisplayManagement;
using Orchard.Environment;
using Orchard.Forms.Services;
using Orchard.Localization;
using Orchard.UI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.ClientSideEditors.Filters.Forms
{
    public class ClientSideLayoutFormHanlder : FormHandler
    {
        private readonly Work<IResourceManager> _resourceManager;
        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public ClientSideLayoutFormHanlder(
            IShapeFactory shapeFactory,
            Work<IResourceManager> resourceManager)
        {
            _resourceManager = resourceManager;
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public override void Built(BuildingContext context) {

            // need to determine that the current form is suitable for client side
            // it can be defined by form name
            // but form name is not accessible from here and i can get form name only for my form
            string formId = context.Shape.Id;
            var avaliableFromIds = new string[] { "GridLayout", "ListLayout", "RawLayout", "ShapeLayout" };
            if (!avaliableFromIds.Contains(formId)) { return; }

            context.Shape._CsFs1(Shape.FieldSet(
                Id: "client-side-switcher-container",
                Title: T("Client side options"),
                _ClientSideSwitcher: Shape.Checkbox(
                    Id: "client-side-switcher", Name: ClientSideFilterFormHelper.Switcher,
                    Title: T("For client side"),
                    Checked: false, Value: "true",
                    Description: T("Check to show it on client side search form as parameter.")
                    )
                )
            );

            context.Shape._CsFs2(Shape.FieldSet(
                Classes: new[] { "client-side-options" },
                _ClientSideName: Shape.TextBox(
                    Id: "client-side-name", Name: ClientSideFilterFormHelper.Name,
                    Title: T("Client side name"),
                    Checked: false, // Value: "Need suggest default name for client side filter. Now i can only parse it from query string.",
                    Description: T("This name will be displayed in query string.")
                    )
                )
            );

            _resourceManager.Value.Require("script", "jQuery");
            _resourceManager.Value.Include("script",
                "~/Modules/MainBit.Projections.ClientSide/Scripts/mainbit-projection-clientside-editor.js",
                "~/Modules/MainBit.Projections.ClientSide/Scripts/mainbit-projection-clientside-editor.js");
        }

        public override void Validating(ValidatingContext context)
        {
            var isForClientSide = context.ValueProvider.GetValue(ClientSideFilterFormHelper.Switcher);
            if (isForClientSide == null || Convert.ToBoolean(isForClientSide.AttemptedValue) == false) { return; }

            var name = context.ValueProvider.GetValue(ClientSideFilterFormHelper.Name);
            if (name == null || String.IsNullOrWhiteSpace(name.AttemptedValue))
            {
                context.ModelState.AddModelError(ClientSideFilterFormHelper.Name, T("The field {0} is required.", T("Client side name").Text).Text);
            }

            if (name.AttemptedValue.ToLower() == ClientSideSortService.QueryStringParamName)
            {
                context.ModelState.AddModelError(ClientSideFilterFormHelper.Name, T("The field {0} can not be equals to Sort.", T("Client side name").Text).Text);
            }

            if (name.AttemptedValue.ToLower() == ClientSideLayoutService.QueryStringParamName)
            {
                context.ModelState.AddModelError(ClientSideFilterFormHelper.Name, T("The field {0} can not be equals to Layout.", T("Client side name").Text).Text);
            }
        }
    }
}