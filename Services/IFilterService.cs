using Orchard;
using Orchard.Data;
using Orchard.Projections.Models;
using System.Linq;

namespace MainBit.Projections.ClientSide.Services
{
    public interface IFilterService : IDependency {
        void MoveUp(int sortId);
        void MoveDown(int sortId);
    }

    public class FilterService : IFilterService
    {
        private readonly IRepository<FilterRecord> _repository;

        public FilterService(IRepository<FilterRecord> repository)
        {
            _repository = repository;
        }

        public void MoveUp(int propertyId)
        {
            var property = _repository.Get(propertyId);

            // look for the previous action in order in same query
            var previous = _repository.Table
                .Where(x => x.Position < property.Position && x.FilterGroupRecord.Id == property.FilterGroupRecord.Id)
                .OrderByDescending(x => x.Position)
                .FirstOrDefault();

            // nothing to do if already at the top
            if (previous == null)
            {
                return;
            }

            // switch positions
            var temp = previous.Position;
            previous.Position = property.Position;
            property.Position = temp;
        }

        public void MoveDown(int propertyId)
        {
            var property = _repository.Get(propertyId);

            // look for the next action in order in same query
            var next = _repository.Table
                .Where(x => x.Position > property.Position && x.FilterGroupRecord.Id == property.FilterGroupRecord.Id)
                .OrderBy(x => x.Position)
                .FirstOrDefault();

            // nothing to do if already at the end
            if (next == null)
            {
                return;
            }

            // switch positions
            var temp = next.Position;
            next.Position = property.Position;
            property.Position = temp;
        }
    }
}
