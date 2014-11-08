using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Orchard.Localization;
using Orchard.Projections.Descriptors.SortCriterion;
using Orchard.Projections.Services;
using Orchard.Utility.Extensions;
using Orchard.Tokens;

namespace MainBit.Projections.ClientSide.Providers.SortCriteria
{
    public class MemberBindingSortCriteria : ISortCriterionProvider {
        private readonly IEnumerable<IMemberBindingProvider> _bindingProviders;
        private readonly ITokenizer _tokenizer;

        public MemberBindingSortCriteria(IEnumerable<IMemberBindingProvider> bindingProviders,
            ITokenizer tokenizer)
        {
            _bindingProviders = bindingProviders;
            _tokenizer = tokenizer;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Describe(DescribeSortCriterionContext describe) {
            var builder = new BindingBuilder();

            foreach(var bindingProvider in _bindingProviders) {
                bindingProvider.GetMemberBindings(builder);
            }

            var groupedMembers = builder.Build().GroupBy(b => b.Property.ReflectedType).ToDictionary(b => b.Key, b => b);

            foreach (var typeMembers in groupedMembers.Keys) {
                var descriptor = describe.For("Variable" + typeMembers.Name, new LocalizedString("Variable " + typeMembers.Name.CamelFriendly()), T("Variable Members for {0}", typeMembers.Name));
                foreach (var member in groupedMembers[typeMembers]) {
                    var closureMember = member;
                    descriptor.Element(member.Property.Name, member.DisplayName, member.Description,
                        context => ApplyFilter(context, closureMember.Property),
                        context => DisplayVariableSortCriterionHelper.DisplaySortCriterion(context, closureMember.DisplayName.Text, T),
                        VariableSortCriterionFormProvider.FormName
                    );
                }
            }
        }

        public void ApplyFilter(SortCriterionContext context, PropertyInfo property) {

            var sort = DisplayVariableSortCriterionHelper.GetSortDirection(context, _tokenizer);

            if (sort == SortDirection.None)
            {
                return;
            }

            context.Query = sort == SortDirection.Ascending
                ? context.Query.OrderBy(alias => alias.ContentPartRecord(property.DeclaringType), x => x.Asc(property.Name))
                : context.Query.OrderBy(alias => alias.ContentPartRecord(property.DeclaringType), x => x.Desc(property.Name));
        }
    }
}