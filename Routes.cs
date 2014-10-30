using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace MainBit.Projections.ClientSide {
    public class Routes : IRouteProvider {

        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "MainBit.Projections.ClientSide/FastSearch/{Id}",
                                                         new RouteValueDictionary {
                                                                                      {"area", "MainBit.Projections.ClientSide"},
                                                                                      {"controller", "FastSearch"},
                                                                                      {"action", "Index"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", "MainBit.Projections.ClientSide"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 }
                         };
        }
    }
}