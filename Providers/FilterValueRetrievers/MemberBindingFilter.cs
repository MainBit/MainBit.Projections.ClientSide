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
using System.Reflection;

namespace MainBit.Projections.ClientSide.Providers.FilterValueRetrievers
{
    public class MemberBindingFilter : IFilterValueRetriverProvider
    {
        private readonly IEnumerable<IMemberBindingProvider> _bindingProviders;

        public MemberBindingFilter(
            IEnumerable<IMemberBindingProvider> bindingProviders)
        {
            _bindingProviders = bindingProviders;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Describe(DescribeFilterContext describe)
        {
            var builder = new BindingBuilder();

            foreach (var bindingProvider in _bindingProviders)
            {
                bindingProvider.GetMemberBindings(builder);
            }

            var groupedMembers = builder.Build().GroupBy(b => b.Property.DeclaringType).ToDictionary(b => b.Key, b => b); // property.DeclaringType in Orchard original

            foreach (var typeMembers in groupedMembers.Keys) {

                var category = "Variable" + typeMembers.Name;
                var descibeFilterFor = describe.For(category);

                foreach(var member in groupedMembers[typeMembers]) {
                    var closureMember = member;
                    var type = member.Property.Name;

                    descibeFilterFor.Element(type, (content) => RetrieveValues(content, closureMember.Property));
                }
            }
        }

        public IEnumerable RetrieveValues(IContent content, PropertyInfo property)
        {
            var fieldValues = new List<dynamic>();
            
            var serchedGenericType = typeof(ContentPart<>).MakeGenericType(property.ReflectedType);
            var retrivedPart = content.As<ContentItem>().Parts.FirstOrDefault(part => {
                var baseType = part.GetType().BaseType;
                if (baseType.IsGenericType && baseType == serchedGenericType)
                {
                    return true;
                }
                return false;
            });

            if (retrivedPart != null)
            {
                var recordPropery = retrivedPart.GetType().GetProperty("Record", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                var record = recordPropery.GetValue(retrivedPart, null);
                //var record = retrivedPart.GetType().BaseType.GetProperty("Record").GetValue(src, null);
                var value = property.GetValue(record, null);
                fieldValues.Add(value);
            }

            return fieldValues;
        }
    }
}