namespace SimulationModel.Model.Item.ItemFactory
{
    public class ItemFactory<T> where T: DefaultQueueItem
    {
        public virtual T CreateItem(double currentTime)
        {
            return (T)new DefaultQueueItem();
        }
    }
}
