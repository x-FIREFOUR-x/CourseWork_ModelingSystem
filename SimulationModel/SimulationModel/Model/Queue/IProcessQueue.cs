using SimulationModel.Model.Item;

namespace SimulationModel.Model.Queue
{
    public interface IProcessQueue<T> where T : DefaultQueueItem
    {
        public int GetSize();

        public T GetItem();

        public bool PutItem(T item);

        public bool CanPutItem();
    }
}
