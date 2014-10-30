using System.Collections.Generic;
using Orchard.Localization;

namespace MainBit.Projections.ClientSide.Descriptors
{
    public class TypeDescriptor<T> {
        public string Category { get; set; }
        public IEnumerable<T> Descriptors { get; set; }
    }
}