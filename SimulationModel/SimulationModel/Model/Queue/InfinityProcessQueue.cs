using System.Collections.Generic;

using SimulationModel.Model.Item;

namespace SimulationModel.Model.Queue
{
    public class InfinityProcessQueue<T> : IProcessQueue<T> where T : DefaultQueueItem
    {
        private List<T> _items;

        public InfinityProcessQueue()
        {
            _items = new List<T>();
        }

        public int GetSize()
        {
            return _items.Count;
        }

        public T GetItem()
        {
            T item = _items[0];
            _items.RemoveAt(0);
            return item;
        }

        public bool PutItem(T item)
        {
            _items.Add(item);
            return true;
        }

        public bool CanPutItem()
        {
            return true;
        }
    }
}
