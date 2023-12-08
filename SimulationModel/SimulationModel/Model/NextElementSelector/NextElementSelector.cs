using System.Collections.Generic;

using SimulationModel.Model.Elements;
using SimulationModel.Model.Item;

namespace SimulationModel.Model.NextElementSelector
{
    public abstract class NextElementSelector<T> where T: DefaultQueueItem
    {
        protected List<(Element<T>, double)> _nextElements;

        public NextElementSelector(List<(Element<T>, double)> nextElements)
        {
            _nextElements = nextElements;
        }

        public abstract Element<T> GetNextElement(DefaultQueueItem item);
    }
}
