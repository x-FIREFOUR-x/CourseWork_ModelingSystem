using System;

using SimulationModel.Model.Queue.Item;

namespace SimulationModel.Model.Queue
{
    public class ItemFactory<T> where T: DefaultQueueItem
    {
        public T CreateItem(double currentTime)
        {
            if (typeof(T) == typeof(Job))
            {
                DefaultQueueItem Job = new Job(currentTime);
                return (T)Job;
            }
            else if (typeof(T) == typeof(DefaultQueueItem))
            {
                return null;
            }
            else
            {
                throw new Exception($"Not realize for type{typeof(T)}");
            }
        }

        
    }
}
