using SimulationModel.Model.Queue.Item;

namespace SimulationModel.Model.Queue.ItemFactory
{
    public class ItemFactory<T> where T: DefaultQueueItem
    {
        public virtual T CreateItem(double currentTime)
        {
            return (T)new DefaultQueueItem();
        }
    }
}
