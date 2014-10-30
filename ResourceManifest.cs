using Orchard.UI.Resources;

namespace MainBit.Projections.ClientSide
{
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {

            var manifest = builder.Add();

            manifest.DefineScript("MainBit.Projections.ClientSide")
                .SetUrl("mainbit-projection-clientside.js", "mainbit-projection-clientside.js").SetVersion("1.0").SetDependencies("jQuery", "jQueryUI_Slider");

            manifest.DefineStyle("MainBit.Projections.ClientSide")
                .SetUrl("mainbit-projection-clientside.css", "mainbit-projection-clientside.css").SetVersion("1.0");

        }
    }
}
