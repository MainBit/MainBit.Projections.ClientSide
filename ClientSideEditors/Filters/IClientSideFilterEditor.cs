using MainBit.Projections.ClientSide.Models.Filters;
using MainBit.Projections.ClientSide.Services;
using MainBit.Projections.ClientSide.Storage;
using Orchard;
using Orchard.Forms.Services;
using Orchard.Projections.Descriptors.Filter;
using Orchard.Projections.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.ClientSideEditors.Filters
{
    public interface IClientSideFilterEditor : IDependency
    {
        string DisplayName { get; }
        bool CanHandle(string filterEditorFormName, string clientSideType = null);
        ClientSideFilter Factory(IStorage clientSideFilterStorage, dynamic state, string filterName, string filterDisplayName, string category, string type);
        void OnFormBuilt(BuildingContext context, dynamic shapeHelper);
        IEnumerable<string> GetNames(ClientSideFilter filter);

        void UpdateAvaliableValues(ClientSideFilter filter, IEnumerable values);
        void ClearAvaliableValues(ClientSideFilter filter);

        NameValueCollection ToQueryString(ClientSideFilter filter);
        void FromQueryString(ClientSideFilter filter, NameValueCollection queryString);
        void BuildTokens(ClientSideFilter filter, IClientSideProjectionTokensService tokenService);
        dynamic BuildDisplay(ClientSideFilter part, dynamic shapeHelper);
        string ToJsonString(ClientSideFilter filter);

        IDictionary<string, string> BuildDefaultState(Orchard.Projections.Descriptors.Filter.FilterDescriptor descriptor);
    }

    public abstract class ClientSideFilterEditor<TClientSideFilter> : IClientSideFilterEditor where TClientSideFilter : ClientSideFilter, new()
    {
        public virtual string DisplayName { get { return typeof(TClientSideFilter).Name; } }
        public virtual bool CanHandle(string filterEditorFormName, string clientSideType = null) { return false; }
        public virtual void OnCreated(TClientSideFilter filter, dynamic state) { }
        public virtual void OnFormBuilt(BuildingContext context, dynamic shapeHelper) { }
        public virtual IDictionary<string, string> BuildDefaultState(Orchard.Projections.Descriptors.Filter.FilterDescriptor descriptor) { return null; }


        ClientSideFilter IClientSideFilterEditor.Factory(IStorage clientSideFilterStorage, dynamic state, string filterName, string filterDisplayName, string category, string type)
        {
            var filter = new TClientSideFilter()
            {
                Name = filterName,
                DisplayName = filterDisplayName,
                Storage = clientSideFilterStorage,
                
                Category = category,
                Type = type,

                Editor = this
            };

            OnCreated(filter, state);

            return filter;
        }


        void IClientSideFilterEditor.UpdateAvaliableValues(ClientSideFilter filter, IEnumerable values) { UpdateAvaliableValues(filter as TClientSideFilter, values); }
        void IClientSideFilterEditor.ClearAvaliableValues(ClientSideFilter filter) { ClearAvaliableValues(filter as TClientSideFilter); }
        IEnumerable<string> IClientSideFilterEditor.GetNames(ClientSideFilter filter) { return GetNames(filter as TClientSideFilter); }
        NameValueCollection IClientSideFilterEditor.ToQueryString(ClientSideFilter filter) { return ToQueryString(filter as TClientSideFilter); }
        void IClientSideFilterEditor.FromQueryString(ClientSideFilter filter, NameValueCollection queryString) { FromQueryString(filter as TClientSideFilter, queryString); }
        void IClientSideFilterEditor.BuildTokens(ClientSideFilter filter, IClientSideProjectionTokensService tokenService) { BuildTokens(filter as TClientSideFilter, tokenService); }
        dynamic IClientSideFilterEditor.BuildDisplay(ClientSideFilter filter, dynamic shapeHelper)
        {
            var result = Display(filter as TClientSideFilter, shapeHelper);

            if (result != null)
            {
                result.Filter = filter;
            }

            return result;
        }
        string IClientSideFilterEditor.ToJsonString(ClientSideFilter filter) { return ToJsonString(filter as TClientSideFilter); }


        protected virtual void UpdateAvaliableValues(TClientSideFilter filter, IEnumerable values) {

        }
        protected virtual void ClearAvaliableValues(TClientSideFilter filter)
        {

        }
        protected virtual IEnumerable<string> GetNames(TClientSideFilter filter)
        {
            return new List<string> { filter.Name };
        }
        protected virtual NameValueCollection ToQueryString(TClientSideFilter filter)
        {
            return HttpUtility.ParseQueryString(string.Empty);
        }
        protected virtual void FromQueryString(TClientSideFilter filter, NameValueCollection queryString)
        {

        }
        protected virtual void BuildTokens(TClientSideFilter filter, IClientSideProjectionTokensService tokenService)
        {

        }
        protected virtual dynamic Display(TClientSideFilter part, dynamic shapeHelper) {
            return null;
        }
        protected virtual string ToJsonString(TClientSideFilter filter)
        {
            return string.Empty;
        }
    }
}