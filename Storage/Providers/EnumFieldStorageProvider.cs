using Orchard.ContentManagement.FieldStorage;
using Orchard.ContentManagement.MetaData;
using Orchard.Fields.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.Storage.Providers
{
    public class EnumFieldStorageProvider : IStorageProvider
    {
        public int Priority
        {
            get { return 1; }
        }
        public string ProviderName
        {
            get { return "EnumerationField"; }
        }

        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IFieldStorageProviderSelector _fieldStorageProviderSelector;

        public EnumFieldStorageProvider(IContentDefinitionManager contentDefinitionManager,
            IFieldStorageProviderSelector fieldStorageProviderSelector)
        {
            _contentDefinitionManager = contentDefinitionManager;
            _fieldStorageProviderSelector = fieldStorageProviderSelector;
        }
        public bool CanHandle(string filterCategory, string filterType)
        {
            var typeStr = filterType.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (typeStr.Length < 2) return false;

            var partDefinition = _contentDefinitionManager.GetPartDefinition(typeStr[0]);
            if (partDefinition == null) { return false; }

            var partFieldDefinition = partDefinition.Fields.FirstOrDefault(p => p.Name == typeStr[1]);
            if (partFieldDefinition == null) { return false; }

            return partFieldDefinition.FieldDefinition.Name == "EnumerationField";
        }

        public IStorage BindStorage(Models.ClientSideProjectionPart part, string filterCategory, string filterType)
        {
            var typeStr = filterType.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            var partDefinition = _contentDefinitionManager.GetPartDefinition(typeStr[0]);
            var partFieldDefinition = partDefinition.Fields.FirstOrDefault(p => p.Name == typeStr[1]);
            var settings = partFieldDefinition.Settings.GetModel<EnumerationFieldSettings>();

            return new TypedStorage(
                (name, valueType) => Get(name, settings),
                (name, valueType, value) => Set(name, value));
        }

        private static object Get(string valueName, EnumerationFieldSettings settings)
        {
            if (valueName == null)
            {
                string[] options = (!String.IsNullOrWhiteSpace(settings.Options)) ? settings.Options.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries) : new string[] { };

                return options;
            }

            return null;
        }

        private static void Set(string valueName, object value)
        {

        }
    }
}