using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Projections.Descriptors.Filter;
using Orchard.Projections.Services;
using Orchard.Utility.Extensions;
using MainBit.Projections.ClientSide.FilterEditors;

namespace MainBit.Projections.ClientSide.Providers.Filters {
    public class MemberBindingFilter : IFilterProvider {
        private readonly IEnumerable<IMemberBindingProvider> _bindingProviders;
        private readonly IVariableFilterCoordinator _filterCoordinator;

        public MemberBindingFilter(
            IEnumerable<IMemberBindingProvider> bindingProviders,
            IVariableFilterCoordinator filterCoordinator)
        {
            _bindingProviders = bindingProviders;
            _filterCoordinator = filterCoordinator;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Describe(DescribeFilterContext describe) {
            var builder = new BindingBuilder();

            foreach(var bindingProvider in _bindingProviders) {
                bindingProvider.GetMemberBindings(builder);
            }

            var groupedMembers = builder.Build().GroupBy(b => b.Property.ReflectedType).ToDictionary(b => b.Key, b => b); // property.DeclaringType in Orchard original

            foreach (var typeMembers in groupedMembers.Keys) {
                var descriptor = describe.For("Variable" + typeMembers.Name, new LocalizedString("Variable " + typeMembers.Name.CamelFriendly()), T("Variable Members for {0}", typeMembers.Name));
                foreach(var member in groupedMembers[typeMembers]) {
                    var closureMember = member;
                    string formName = _filterCoordinator.GetForm(closureMember.Property.PropertyType);
                    descriptor.Element(member.Property.Name, member.DisplayName, member.Description,
                        context => ApplyFilter(context, closureMember.Property),
                        context => _filterCoordinator.Display(closureMember.Property.PropertyType, closureMember.DisplayName.Text, context.State),
                        formName
                    );
                }
            }
        }

        public void ApplyFilter(FilterContext context, PropertyInfo property) {
            var predicate = _filterCoordinator.Filter(property.PropertyType, property.Name, context.State);
            if (predicate == null) { return; }
            Action<IAliasFactory> alias = x => x.ContentPartRecord(property.ReflectedType); // property.DeclaringType in Orchard original
            context.Query = context.Query.Where(alias, predicate);
        }
    }
}