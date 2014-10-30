using Orchard.Events;
using MainBit.Projections.ClientSide.Descriptors.FilterValueRetrievers;

namespace MainBit.Projections.ClientSide.Services
{
    public interface IFilterValueRetriverProvider : IEventHandler
    {
        void Describe(DescribeFilterContext describe);
    }
}