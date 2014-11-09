using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Forms.Services;
using Orchard.Localization;
using Orchard.Projections.Models;
using Orchard.Projections.Services;
using Orchard.Projections.ViewModels;
using Orchard.Security;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Orchard;
using Orchard.Projections;
using MainBit.Projections.ClientSide.ClientSideEditors.Filters;
using System.Collections.Generic;
using System;
using MainBit.Projections.ClientSide.Services;

namespace MainBit.Projections.ClientSide.Controllers {
    [ValidateInput(false), Admin]
    public class FilterController : Controller {
        public FilterController(
            IOrchardServices services,
            IFormManager formManager,
            IShapeFactory shapeFactory,
            IProjectionManager projectionManager,
            IRepository<FilterRecord> repository,
            IRepository<FilterGroupRecord> groupRepository,
            IQueryService queryService,
            IClientSideFilterEditorSelector clientSideFilterEditorSelector,
            IFilterService filterService)
        {
            Services = services;
            _formManager = formManager;
            _projectionManager = projectionManager;
            _repository = repository;
            _groupRepository = groupRepository;
            _queryService = queryService;
            Shape = shapeFactory;
            _clientSideFilterEditorSelector = clientSideFilterEditorSelector;
            _filterService = filterService;
        }

        public IOrchardServices Services { get; set; }
        private readonly IFormManager _formManager;
        private readonly IProjectionManager _projectionManager;
        private readonly IRepository<FilterRecord> _repository;
        private readonly IRepository<FilterGroupRecord> _groupRepository;
        private readonly IQueryService _queryService;
        private readonly IClientSideFilterEditorSelector _clientSideFilterEditorSelector;
        private readonly IFilterService _filterService;
        public Localizer T { get; set; }
        public dynamic Shape { get; set; }

        public ActionResult Add(int id) {
            if (!Services.Authorizer.Authorize(Permissions.ManageQueries, T("Not authorized to manage queries")))
                return new HttpUnauthorizedResult();

            var viewModel = new FilterAddViewModel { Id = id, Filters = _projectionManager.DescribeFilters() };
            return View(viewModel);
        }

        public ActionResult AddDefaultClientSide(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageQueries, T("Not authorized to manage queries")))
                return new HttpUnauthorizedResult();

            var typeDescriptors = _projectionManager.DescribeFilters().ToList();

            for (var i = typeDescriptors.Count - 1; i >= 0; i--)
            {
                var typeDescriptor = typeDescriptors[i];
                typeDescriptor.Descriptors = typeDescriptor.Descriptors.Where(d => _clientSideFilterEditorSelector.GetEditor(d.Form) != null);
                if (!typeDescriptor.Descriptors.Any())
                {
                    typeDescriptors.Remove(typeDescriptor);
                }
            }

            var viewModel = new FilterAddViewModel { Id = id, Filters = typeDescriptors };
            return View(viewModel);
        }

        public ActionResult AddGroup(int id) {
            if (!Services.Authorizer.Authorize(Permissions.ManageQueries, T("Not authorized to manage queries")))
                return new HttpUnauthorizedResult();

            var query = _queryService.GetQuery(id).Record;

            if (query == null) {
                return HttpNotFound();
            }

            query.FilterGroups.Add( new FilterGroupRecord());

            return RedirectToAction("Edit", "Admin", new { query.ContentItemRecord.Id });
        }

        [HttpPost]
        public ActionResult DeleteGroup(int id) {
            if (!Services.Authorizer.Authorize(Permissions.ManageQueries, T("Not authorized to manage queries")))
                return new HttpUnauthorizedResult();

            var group = _groupRepository.Get(id);

            if (group == null) {
                return HttpNotFound();
            }
            var queryId = group.QueryPartRecord.Id;

            group.QueryPartRecord.FilterGroups.Remove(group);
            _groupRepository.Delete(group);

            return RedirectToAction("Edit", "Admin", new { id = queryId });
        }


        public ActionResult Delete(int id, int filterId) {
            if (!Services.Authorizer.Authorize(Permissions.ManageQueries, T("Not authorized to manage queries")))
                return new HttpUnauthorizedResult();

            var filter = _repository.Get(filterId);
            if(filter == null) {
                return HttpNotFound();
            }

            filter.FilterGroupRecord.Filters.Remove(filter);
            _repository.Delete(filter);

            Services.Notifier.Information(T("Filter deleted"));

            return RedirectToAction("Edit", "Admin", new { id });
        }

        public ActionResult EditDefaultClientSide(int id, string category, string type)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageQueries, T("Not authorized to manage queries")))
                return new HttpUnauthorizedResult();

            var filter = _projectionManager.DescribeFilters().SelectMany(x => x.Descriptors).FirstOrDefault(x => x.Category == category && x.Type == type);
            
            if (filter == null) {
                return HttpNotFound();
            }

            var clientSideFilterEditor = _clientSideFilterEditorSelector.GetEditor(filter.Form);

            if (clientSideFilterEditor == null)
            {
                return HttpNotFound();
            }

            var defaultState = clientSideFilterEditor.BuildDefaultState(filter);
            
            if (defaultState == null)
            {
                return HttpNotFound();
            }

            var group = _groupRepository.Get(id);
            var filterRecord = new FilterRecord
            {
                Category = category,
                Type = type,
                Position = group.Filters.Count,
                State = FormParametersHelper.ToString(defaultState),
                Description = defaultState["Description"]
            };
            group.Filters.Add(filterRecord);

            return RedirectToAction("Edit", "Admin", new { group.QueryPartRecord.Id });
        }

        public ActionResult Edit(int id, string category, string type, int filterId = -1) {
            if (!Services.Authorizer.Authorize(Permissions.ManageQueries, T("Not authorized to manage queries")))
                return new HttpUnauthorizedResult();

            var filter = _projectionManager.DescribeFilters().SelectMany(x => x.Descriptors).FirstOrDefault(x => x.Category == category && x.Type == type);

            if (filter == null) {
                return HttpNotFound();
            }

            // build the form, and let external components alter it
            var form = filter.Form == null ? null : _formManager.Build(filter.Form);

            string description = "";

            // bind form with existing values).
            if (filterId != -1) {
                var group = _groupRepository.Get(id);
                var filterRecord = group.Filters.FirstOrDefault(f => f.Id == filterId);
                if (filterRecord != null) {
                    description = filterRecord.Description;
                    var parameters = FormParametersHelper.FromString(filterRecord.State);
                    _formManager.Bind(form, new DictionaryValueProvider<string>(parameters, CultureInfo.InvariantCulture));
                }
            }

            var viewModel = new FilterEditViewModel { Id = id, Description = description, Filter = filter, Form = form };
            return View(viewModel);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPost(int id, string category, string type, [DefaultValue(-1)] int filterId, FormCollection formCollection) {
            var group = _groupRepository.Get(id);

            var filter = _projectionManager.DescribeFilters().SelectMany(x => x.Descriptors).Where(x => x.Category == category && x.Type == type).FirstOrDefault();

            var model = new FilterEditViewModel();
            TryUpdateModel(model);

            // validating form values
            _formManager.Validate(new ValidatingContext { FormName = filter.Form, ModelState = ModelState, ValueProvider = ValueProvider });

            if (ModelState.IsValid) {
                var filterRecord = group.Filters.Where(f => f.Id == filterId).FirstOrDefault();

                // add new filter record if it's a newly created filter
                if (filterRecord == null) {
                    filterRecord = new FilterRecord {
                        Category = category, 
                        Type = type, 
                        Position = group.Filters.Count
                    };
                    group.Filters.Add(filterRecord);
                }

                var dictionary = formCollection.AllKeys.ToDictionary(key => key, formCollection.Get);

                // save form parameters
                filterRecord.State = FormParametersHelper.ToString(dictionary);
                filterRecord.Description = model.Description;

                return RedirectToAction("Edit", "Admin", new { group.QueryPartRecord.Id });
            }

            // model is invalid, display it again
            var form = _formManager.Build(filter.Form);

            _formManager.Bind(form, formCollection);
            var viewModel = new FilterEditViewModel { Id = id, Description = model.Description, Filter = filter, Form = form };

            return View(viewModel);
        }

        public ActionResult Move(string direction, int id, int queryId)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageQueries, T("Not authorized to manage queries")))
                return new HttpUnauthorizedResult();

            switch (direction)
            {
                case "up": _filterService.MoveUp(id);
                    break;
                case "down": _filterService.MoveDown(id);
                    break;
                default:
                    throw new ArgumentException("direction");
            }

            return RedirectToAction("Edit", "Admin", new { id = queryId });
        }
    }
}