using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Localization;
using MainBit.Projections.ClientSide.Descriptors.FilterValueRetrievers;
using Orchard.Projections.FieldTypeEditors;
using Orchard.Projections.Services;
using Orchard.Utility.Extensions;
using MainBit.Projections.ClientSide.FieldTypeEditors;
using Orchard.ContentManagement.FieldStorage;
using System.Collections;
using MainBit.Projections.ClientSide.Services;

namespace MainBit.Projections.ClientSide.Providers.FilterValueRetrievers
{
    public class ContentFieldsFilter : IFilterValueRetriverProvider
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IEnumerable<IContentFieldDriver> _contentFieldDrivers;
        private readonly IEnumerable<IVariableFieldTypeEditor> _fieldTypeEditors;
        private readonly IFieldStorageProvider _fieldStorageProvider;

        public ContentFieldsFilter(
            IContentDefinitionManager contentDefinitionManager,
            IEnumerable<IContentFieldDriver> contentFieldDrivers,
            IEnumerable<IVariableFieldTypeEditor> fieldTypeEditors,
            IFieldStorageProvider fieldStorageProvider,
            IProjectionManager projectionManager)
        {
            _contentDefinitionManager = contentDefinitionManager;
            _contentFieldDrivers = contentFieldDrivers;
            _fieldTypeEditors = fieldTypeEditors;
            _fieldStorageProvider = fieldStorageProvider;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Describe(DescribeFilterContext describe)
        {
            foreach (var part in _contentDefinitionManager.ListPartDefinitions())
            {
                if (!part.Fields.Any())
                {
                    continue;
                }

                var category = "Variable" + part.Name + "ContentFields";

                foreach (var field in part.Fields)
                {
                    var localField = field;
                    var localPart = part;
                    var drivers = _contentFieldDrivers.Where(x => x.GetFieldInfo().Any(fi => fi.FieldTypeName == localField.FieldDefinition.Name)).ToList();

                    string type = null;
                    var membersContextForGetType = new DescribeMembersContext(
                        (storageName, storageType, displayName, description) =>
                        {
                            IVariableFieldTypeEditor fieldTypeEditor = _fieldTypeEditors.FirstOrDefault(x => x.CanHandle(storageType));

                            if (fieldTypeEditor == null)
                            {
                                return;
                            }

                            type = localPart.Name + "." + localField.Name + "." + storageName;
                        });
                    foreach (var driver in drivers)
                    {
                        driver.Describe(membersContextForGetType);
                    }
                    if(type == null) {
                        continue;
                    }

                    var descibeFilterFor = describe.For(category);
                    descibeFilterFor.Element(type, (content) => RetrieveValues(content, part.Name, field, drivers));
                }
            }
        }

        public IEnumerable RetrieveValues(IContent content, string partName, ContentPartFieldDefinition field, IEnumerable<IContentFieldDriver> drivers)
        {
            var retrivedPart = content.As<ContentItem>().Parts.FirstOrDefault(p => p.PartDefinition.Name == partName);
            var fieldStorage = _fieldStorageProvider.BindStorage(retrivedPart, field);

            var fieldValues = new List<dynamic>();
            var membersContext = new DescribeMembersContext(fieldStorage, values =>
            {
                foreach (var value in values)
                {
                    if (value == null)
                    {
                        continue;
                    }
                    fieldValues.Add(value);
                }
            });

            foreach (var driver in drivers)
            {
                driver.Describe(membersContext);
            }
            return fieldValues;
        }
    }
}