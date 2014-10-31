using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.Services
{
    public interface IClientSideProjectionTokensService : IDependency
    {
        object GetValue(string name);
        void SetValue(string name, string value);
        void RemoveValue(string name);
    }

    public class ClientSideProjectionTokensService : IClientSideProjectionTokensService
    {
        public const string TokenName = "ClientSideFilters.Value:{0}";
        private const string Name = "ClientSideProjectionTokens";
        private readonly IWorkContextAccessor _wca;

        public ClientSideProjectionTokensService(IWorkContextAccessor wca)
        {
            _wca = wca;
        }

        public object GetValue(string name)
        {
            return Values.ContainsKey(name) ? Values[name] : null;
        }

        public void SetValue(string name, string value)
        {
            Values[name] = value;
        }

        public void RemoveValue(string name)
        {
            if (Values.ContainsKey(name))
            {
                Values.Remove(name);
            }
        }

        private IDictionary<string, object> _values = null;
        private IDictionary<string, object> Values
        {
            get
            {
                if (!_wca.GetContext().HttpContext.Items.Contains(Name))
                {
                    _wca.GetContext().HttpContext.Items[Name] = new Dictionary<string, object>(); 
                }
                return _wca.GetContext().HttpContext.Items[Name] as Dictionary<string, object>;

                // need one instance per request, i dont what about IDependecy
                //if (_values == null)
                //{
                //    _values = new Dictionary<string, object>();
                //}
                //return _values;
            }
        }

    }
}