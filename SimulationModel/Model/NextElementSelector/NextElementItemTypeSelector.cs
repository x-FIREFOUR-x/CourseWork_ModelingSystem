using System.Collections.Generic;
using System.Linq;

using SimulationModel.Model.Elements;
using SimulationModel.Model.Queue.Item;

namespace SimulationModel.Model.NextElementSelector
{
    public class NextElementItemTypeSelector<T> : NextElementSelector<T> where T : DefaultQueueItem
    {
        public NextElementItemTypeSelector(List<(Element<T>, double)> nextElements)
           : base(nextElements)
        {
        }

        public override Element<T> GetNextElement(DefaultQueueItem item)
        {
            if(item is Job Job)
            {
                return _nextElements.FirstOrDefault(c => (int)(c.Item2) == Job.Type).Item1;
            }  

            return null;
        }
    }
}
