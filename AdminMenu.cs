using Orchard.Localization;
using Orchard.Projections;
using Orchard.Security;
using Orchard.UI.Navigation;

namespace MainBit.Projections.ClientSide
{
    public class AdminMenu : INavigationProvider {
        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder) {

            builder.AddImageSet("projector").Add(T("Queries"), "3",
               menu => menu
                   .Add(T("Client side queries"), "3.0",
                       qi => qi.Action("Index", "Admin", new { area = "MainBit.Projections.ClientSide" }).Permission(Permissions.ManageQueries).LocalNav())
            );
        }
    }
}
