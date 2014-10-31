using MainBit.Projections.ClientSide.Models.SortCriteria;
using MainBit.Projections.ClientSide.Services;
using Orchard;
using Orchard.Forms.Services;
using Orchard.Projections.Providers.SortCriteria;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.ClientSideEditors.SortCriteria
{
    public interface IClientSideSortCriterionEditor : IDependency
    {
        bool CanHandle(string sortCriterionFormName);
        ClientSideSortCriterion Factory(IDictionary<string, string> state, string sortCriterionName, string sortCriterionDisplayName, string category, string type);
        void OnFormBuilt(BuildingContext context, dynamic shapeHelper);


        void BuildTokens(ClientSideSortCriterion sortCriterion, IClientSideProjectionTokensService tokenService);

        dynamic BuildDisplay(ClientSideSortCriterion part, dynamic shapeHelper);

        IDictionary<string, string> BuildDefaultState(Orchard.Projections.Descriptors.SortCriterion.SortCriterionDescriptor descriptor);
    }

    public abstract class ClientSideSortCriterionEditor<TClientSideSortCriterion> : IClientSideSortCriterionEditor where TClientSideSortCriterion : ClientSideSortCriterion, new()
    {
        
        public virtual bool CanHandle(string filterEditorFormName) { return false; }
        protected virtual void OnCreated(TClientSideSortCriterion sortCriterion, IDictionary<string, string> state) { }
        public virtual void OnFormBuilt(BuildingContext context, dynamic shapeHelper) { }
        public virtual IDictionary<string, string> BuildDefaultState(Orchard.Projections.Descriptors.SortCriterion.SortCriterionDescriptor descriptor) { return null; }

        ClientSideSortCriterion IClientSideSortCriterionEditor.Factory(IDictionary<string, string> state, string sortCriterionName, string sortCriterionDisplayName, string category, string type)
        {
            var sortCriterion = new TClientSideSortCriterion()
            {
                Name = sortCriterionName,
                DisplayName = sortCriterionDisplayName,

                Category = category,
                Type = type,

                Editor = this
            };

            OnCreated(sortCriterion, state);

            return sortCriterion;
        }


        void IClientSideSortCriterionEditor.BuildTokens(ClientSideSortCriterion sortCriterion, IClientSideProjectionTokensService tokenService)
        {
            BuildTokens(sortCriterion as TClientSideSortCriterion, tokenService);
        }
        dynamic IClientSideSortCriterionEditor.BuildDisplay(ClientSideSortCriterion sortCriterion, dynamic shapeHelper)
        {
            var result = Display(sortCriterion as TClientSideSortCriterion, shapeHelper);

            if (result != null)
            {
                result.SortCriterion = sortCriterion;
            }

            return result;
        }




        protected virtual void BuildTokens(TClientSideSortCriterion sortCriterion, IClientSideProjectionTokensService tokenService)
        {

        }
        protected virtual dynamic Display(TClientSideSortCriterion sortCriterion, dynamic shapeHelper)
        {
            return null;
        }
    }
}