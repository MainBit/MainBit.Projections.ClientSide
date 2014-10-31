using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.ClientSideEditors.SortCriteria
{
    public interface IClientSideSortEditorSelector : IDependency
    {
        IClientSideSortCriterionEditor GetEditor(string sortCriterionFormName);
    }

    public class ClientSideSortEditorSelector : IClientSideSortEditorSelector
    {
        private readonly IEnumerable<IClientSideSortCriterionEditor> _editors;

        public ClientSideSortEditorSelector(IEnumerable<IClientSideSortCriterionEditor> editors)
        {
            _editors = editors;
        }

        public IClientSideSortCriterionEditor GetEditor(string sortCriterionFormName)
        {
            return _editors.FirstOrDefault(e => e.CanHandle(sortCriterionFormName));
        }
    }
}