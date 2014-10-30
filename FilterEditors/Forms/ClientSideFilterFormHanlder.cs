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
    public class ClientSideFilterFormHanlder : FormHandler
    {
        private readonly IClientSideFilterEditorSelector _clientSideFilterEditorSelector;
        private readonly IClientSideSortCriterionEditorSelector _clientSideSortCriterionEditorSelector;

        private readonly Work<IResourceManager> _resourceManager;
        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public ClientSideFilterFormHanlder(
            IClientSideFilterEditorSelector clientSideFilterEditorSelector,
            IClientSideSortCriterionEditorSelector clientSideSortCriterionEditorSelector,
            IShapeFactory shapeFactory,
            Work<IResourceManager> resourceManager)
        {
            _clientSideFilterEditorSelector = clientSideFilterEditorSelector;
            _clientSideSortCriterionEditorSelector = clientSideSortCriterionEditorSelector;
            _resourceManager = resourceManager;
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public override void Built(BuildingContext context) {

            // need to determine that the current form is suitable for client side
            // it can be defined by form name
            // but form name is not accessible from here and i can get form name only for my form
            var formName = context.Shape.FormName;
            IClientSideFilterEditor filterEditor = _clientSideFilterEditorSelector.GetEditor(formName);
            IClientSideSortCriterionEditor sortCriterionEditor = _clientSideSortCriterionEditorSelector.GetEditor(formName);
            if (filterEditor == null && sortCriterionEditor == null) { return; }

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

            if (filterEditor != null)
            {
                filterEditor.OnFormBuilt(context, Shape);
            }
            if (sortCriterionEditor != null)
            {
                sortCriterionEditor.OnFormBuilt(context, Shape);
            }

            _resourceManager.Value.Require("script", "jQuery");
            _resourceManager.Value.Include("script",
                "~/Modules/MainBit.Projections.ClientSide/Scripts/mainbit-projection-clientside-editor.js",
                "~/Modules/MainBit.Projections.ClientSide/Scripts/mainbit-projection-clientside-editor.js");
        }

        public override void Validating(ValidatingContext context)
        {
            var filterEditor = _clientSideFilterEditorSelector.GetEditor(context.FormName);
            var sortCriterionEditor = _clientSideSortCriterionEditorSelector.GetEditor(context.FormName);
            if (filterEditor == null && sortCriterionEditor == null) { return; }

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