using MainBit.Projections.ClientSide.Services;
using Orchard;
using Orchard.Localization;
using Orchard.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.Tokens
{
    public class ClientSideQueryTokens : ITokenProvider
    {
        private readonly IClientSideProjectionTokensService _clientSideFilterTokensService;

        public ClientSideQueryTokens(IClientSideProjectionTokensService clientSideFilterTokensService)
        {
            _clientSideFilterTokensService = clientSideFilterTokensService;
            T = NullLocalizer.Instance;
        }
        public Localizer T { get; set; }
        
        public void Describe(DescribeContext context) {

            context.For("ClientSideFilters", T("Client side filters"), T("Client side filter tokens."))
                .Token("Value:*", T("Value:<element>"), T("The client side filter value for specified filter."))
            ;
        }
       

        public void Evaluate(EvaluateContext context)
        {
            context.For("ClientSideFilters", _clientSideFilterTokensService)
                .Token(
                    token => token.StartsWith("Value:", StringComparison.OrdinalIgnoreCase) ? token.Substring("Value:".Length) : null,
                    (token, tokenValue) => tokenValue.GetValue(token)
                );
        }
    }
}