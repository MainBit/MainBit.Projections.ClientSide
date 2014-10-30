using System.Collections.Generic;
using System.Linq;
using Orchard.Localization;

namespace MainBit.Projections.ClientSide.Descriptors.FilterValueRetrievers
{
    public class DescribeFilterContext {
        private readonly Dictionary<string, DescribeFilterFor> _describes = new Dictionary<string, DescribeFilterFor>();

        public IEnumerable<TypeDescriptor<FilterDescriptor>> Describe() {
            return _describes.Select(kp => new TypeDescriptor<FilterDescriptor> {
                Category = kp.Key,
                Descriptors = kp.Value.Types
            });
        }

        //public DescribeFilterFor For(string category) {
        //    return For(category);
        //}

        public DescribeFilterFor For(string category) {
            DescribeFilterFor describeFor;
            if (!_describes.TryGetValue(category, out describeFor)) {
                describeFor = new DescribeFilterFor(category);
                _describes[category] = describeFor;
            }
            return describeFor;
        }
    }


}